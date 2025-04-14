using System;

namespace DiceRoll.Input
{
    internal sealed class ShuntingYardOperators
    {
        private readonly ShuntingYardState _state;

        public ShuntingYardOperators(ShuntingYardState state)
        {
            _state = state;
        }

        public void Push(OperatorToken operatorToken, in Substring context) =>
            _state.Operators.Push(in operatorToken, context);

        public bool TryPeek(out OperatorToken operatorToken) =>
            _state.Operators.TryPeek(out operatorToken);
        
        public bool TryPeek(out FormulaToken<DelayedOperatorToken> operatorToken) =>
            _state.DelayedOperators.TryPeek(out operatorToken);

        public bool TryPop(out FormulaToken<OperatorToken> operatorToken) =>
            _state.Operators.TryPop(out operatorToken);

        public FormulaToken<OperatorToken> Pop() =>
            _state.Operators.Pop();

        public void InvokeOperatorOrThrow(in FormulaToken<OperatorToken> operatorToken)
        {
            try
            {
                InvokeOperator(operatorToken.Value.Invoker);
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

        public void InvokeAfterDelayedOperators(in FormulaToken<OperatorToken> invoker)
        {
            InvokeDelayedOperators();
            InvokeOperatorOrThrow(in invoker);
        }

        public void DelayOperatorInvocation(OperatorInvoker invoker, in Substring context) =>
            _state.DelayedOperators.Push(new DelayedOperatorToken(invoker, _state.ParenthesisLevel, _state.Operands.Count), in context);

        private void InvokeOperatorOrThrow(in FormulaToken<DelayedOperatorToken> operatorToken)
        {
            try
            {
                InvokeOperator(operatorToken.Value.Invoker);
            }
            catch (Exception e)
            {
                throw _state.Wrap(in operatorToken, e);
            }
        }

        private void InvokeOperator(OperatorInvoker invoker)
        {
            int arity = invoker.Arity;
            
            if (_state.Operands.Count < arity)
                throw new OperatorInvocationException(ParsingErrorMessages.OperandsExpected(arity, _state.Operands.Count));

            OperandsStackAccess stackAccess = new(_state.Operands, arity);
            
            invoker.Invoke(stackAccess);
        }
    }
}
