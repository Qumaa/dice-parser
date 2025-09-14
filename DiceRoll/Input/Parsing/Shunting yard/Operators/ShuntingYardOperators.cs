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

            OperatorInvoker invoker = operatorToken.Invoker;

            if (invoker.RightArity is 0)
            {
                // invoke immediately using existing operands
                InvokeOperatorOrThrow(in mapped);
                return;
            }

            if (invoker is { RightArity: 1, LeftArity: > 0 })
            {
                // resolve using default shunting-yard mechanism
                _state.Operators.Push(in mapped);
                return;
            }

            // delay until more operands are pushed
            
            // include (LeftArity) more operands in the delayed operator definition to use during its invocation
            int capturedOperands = _state.Operands.Count - invoker.LeftArity;
            
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
                throw _state.MapException(in operatorToken, e);
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
                throw _state.MapException(in operatorToken, e);
            }
        }

        private void InvokeOperator(OperatorInvoker invoker, in Range operatorRange)
        {
            int arity = invoker.Arity;
            
            if (_state.Operands.Count < arity)
                throw new OperatorInvocationException(ParsingErrorMessages.OperandsExpected(arity, _state.Operands.Count));

            OperandsStackAccess access = new(_state.Operands, arity, in operatorRange);
            
            INode result = invoker.Invoke(access);
            
            access.PushResult(result);
        }
    }
}
