using System;

namespace DiceRoll.Input.Parsing
{
    // todo inline the operator invocation struct into this class
    internal sealed class OperatorInvocationHandler
    {
        private readonly ShuntingYardState _state;
        private readonly OperandCastingTable _castingTable;
        
        public OperatorInvocationHandler(ShuntingYardState state, OperandCastingTable castingTable)
        {
            _state = state;
            _castingTable = castingTable;
        }

        public void InvokeOperator(in Mapped<Operator> operatorToken) =>
            InvokeOperatorOrThrow(operatorToken.Value.InvocationBehaviour, in operatorToken.Range);

        public void TryInvokeDelayedOperators()
        {
            while (_state.DelayedOperators.TryPeek(out DelayedOperator token) &&
                   token.CapturedParenthesisLevel >= _state.ParenthesisLevel &&
                   token.CapturedOperands + token.InvocationBehaviour.RightArity <= _state.Operands.Count)
                InvokeOperator(_state.DelayedOperators.Pop());
        }

        public void InvokeAfterDelayedOperators(in Mapped<Operator> invoker)
        {
            TryInvokeDelayedOperators();
            InvokeOperator(in invoker);
        }

        private void InvokeOperator(in Mapped<DelayedOperator> operatorToken) =>
            InvokeOperatorOrThrow(operatorToken.Value.InvocationBehaviour, in operatorToken.Range);
        
        private void InvokeOperatorOrThrow(OperatorInvocationBehaviour invocationBehaviour, in Range operatorMappedRange) =>
            new OperatorInvocation(_state, _castingTable, invocationBehaviour, in operatorMappedRange).Perform();
    }
}
