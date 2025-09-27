using System;

namespace DiceRoll.Input.Parsing
{
    internal sealed class ShuntingYardOperators
    {
        private readonly ShuntingYardState _state;
        private readonly OperandCastingTable _castingTable;

        public ShuntingYardOperators(ShuntingYardState state, OperandCastingTable castingTable)
        {
            _state = state;
            _castingTable = castingTable;
        }

        public void OpenParenthesis(in Substring context) =>
            _state.Operators.MapAndPush(in Operator.OpenParenthesis, context);

        public OperatorProcessingResult Process(in Operator @operator, in Substring context)
        {
            Mapped<Operator> mapped = _state.Mapper.Map(in @operator, in context);

            OperatorInvocationBehaviour invocationBehaviour = @operator.InvocationBehaviour;

            if (invocationBehaviour.RightArity is 0)
            {
                // invoke immediately using existing operands
                InvokeOperator(in mapped);
                return OperatorProcessingResult.Invoked;
            }

            if (invocationBehaviour is { RightArity: 1, LeftArity: > 0 })
            {
                // resolve using default shunting-yard mechanism
                _state.Operators.Push(in mapped);
                return OperatorProcessingResult.Pushed;
            }

            // delay until more operands are pushed
            _state.DelayedOperators.MapAndPush(
                new DelayedOperator(@operator.InvocationBehaviour, _state.ParenthesisLevel, _state.Operands.Count),
                in context
                );

            return OperatorProcessingResult.Delayed;
        }

        public bool TryPeek(out Operator @operator) =>
            _state.Operators.TryPeek(out @operator);
        
        public bool TryPeek(out Mapped<DelayedOperator> operatorToken) =>
            _state.DelayedOperators.TryPeek(out operatorToken);

        public bool TryPop(out Mapped<Operator> operatorToken) =>
            _state.Operators.TryPop(out operatorToken);

        public Mapped<Operator> Pop() =>
            _state.Operators.Pop();

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
