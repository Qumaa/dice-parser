using System;

namespace DiceRoll.Input.Parsing
{
    internal sealed class ShuntingYardOperators
    {
        private readonly ShuntingYardState _state;

        public ShuntingYardOperators(ShuntingYardState state)
        {
            _state = state;
        }

        public void Push(in OperatorToken operatorToken, in Substring context)
        {
            if (operatorToken.Invoker is null)
            {
                _state.Operators.MapAndPush(in operatorToken, context);
                return;
            }

            Mapped<OperatorToken> mapped = _state.Mapper.Map(in operatorToken, in context);
            ArgumentsLayout layout = operatorToken.Invoker.Layout;

            if (layout is ArgumentsLayout.FullLeft)
            {
                InvokeOperatorOrThrow(in mapped);
                return;
            }

            if (layout is ArgumentsLayout.Left)
            {
                _state.Operators.Push(in mapped);
                return;
            }

            int capturedOperands = _state.Operands.Count;
            if (layout is ArgumentsLayout.Right)
                capturedOperands--; // require one more operand
            
            _state.DelayedOperators.MapAndPush(
                new DelayedOperatorToken(operatorToken.Invoker, _state.ParenthesisLevel, capturedOperands),
                in context
                );
        }

        public bool TryPeek(out OperatorToken operatorToken) =>
            _state.Operators.TryPeek(out operatorToken);
        
        public bool TryPeek(out Mapped<DelayedOperatorToken> operatorToken) =>
            _state.DelayedOperators.TryPeek(out operatorToken);

        public bool TryPop(out Mapped<OperatorToken> operatorToken) =>
            _state.Operators.TryPop(out operatorToken);

        public Mapped<OperatorToken> Pop() =>
            _state.Operators.Pop();

        public void InvokeOperatorOrThrow(in Mapped<OperatorToken> operatorToken)
        {
            try
            {
                InvokeOperator(operatorToken.Value.Invoker, in operatorToken.Range);
            }
            catch (Exception e)
            {
                throw _state.Wrap(in operatorToken, e);
            }
        }

        public void InvokeDelayedOperators()
        {
            while (_state.DelayedOperators.TryPeek(out DelayedOperatorToken token) &&
                   token.CapturedParenthesisLevel >= _state.ParenthesisLevel &&
                   token.CapturedOperands + token.Invoker.Arity <= _state.Operands.Count)
                InvokeOperatorOrThrow(_state.DelayedOperators.Pop());
        }

        public void InvokeAfterDelayedOperators(in Mapped<OperatorToken> invoker)
        {
            InvokeDelayedOperators();
            InvokeOperatorOrThrow(in invoker);
        }

        private void InvokeOperatorOrThrow(in Mapped<DelayedOperatorToken> operatorToken)
        {
            try
            {
                InvokeOperator(operatorToken.Value.Invoker, in operatorToken.Range);
            }
            catch (Exception e)
            {
                throw _state.Wrap(in operatorToken, e);
            }
        }

        private void InvokeOperator(OperatorInvoker invoker, in Range operatorRange)
        {
            int arity = invoker.Arity;
            
            if (_state.Operands.Count < arity)
                throw new OperatorInvocationException(ParsingErrorMessages.OperandsExpected(arity, _state.Operands.Count));

            invoker.Invoke(new OperandsStackAccess(_state.Operands, arity, in operatorRange));
        }
    }
}
