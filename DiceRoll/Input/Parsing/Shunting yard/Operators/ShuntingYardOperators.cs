using System;

namespace DiceRoll.Input.Parsing
{
    internal sealed class ShuntingYardOperators
    {
        private readonly ShuntingYardState _state;
        private readonly OperandCastingTable _castingTable;
        
        public OperatorUsageForm CurrentUsageForm => _state.PrecedingTokenKind is TokenKind.Operand ?
                OperatorUsageForm.Infix :
                OperatorUsageForm.Prefix;

        public ShuntingYardOperators(ShuntingYardState state, OperandCastingTable castingTable)
        {
            _state = state;
            _castingTable = castingTable;
        }

        public void OpenParenthesis(in Substring substring)
        {
            _state.Operators.MapAndPush(in Operator.OpenParenthesis, substring);
            _state.Annotate().ParenthesisOpening();
        }

        public void CloseParenthesis()
        {
            if (_state.ClosingParenthesisWouldImposeImbalance)
                throw new UnbalancedParenthesisException();

            while (TryPop(out Mapped<Operator> operatorToken))
            {
                if (operatorToken.Value.IsOpenParenthesis)
                    break;

                InvokeOperator(in operatorToken);
            }
            
            _state.Annotate().ParenthesisClosing();
            
            TryInvokeDelayedOperators();
        }

        public void Process(in Operator @operator, in Substring substring)
        {
            InvokeHigherPrecedenceOperators(in @operator);
            
            Mapped<Operator> mapped = _state.Mapper.Map(in @operator, in substring);

            OperatorInvocationBehaviour invocationBehaviour = @operator.InvocationBehaviour;

            if (invocationBehaviour.RightArity is 0)
            {
                // invoke immediately using existing operands
                InvokeOperator(in mapped);
                _state.Annotate().OperandProcessing();
                return;
            }
            
            _state.Annotate().OperatorProcessing();

            if (invocationBehaviour is { RightArity: 1, LeftArity: > 0 })
            {
                // resolve using default shunting-yard mechanism
                _state.Operators.Push(in mapped);
                return;
            }

            // delay until more operands are pushed
            _state.DelayedOperators.MapAndPush(
                new DelayedOperator(@operator.InvocationBehaviour, _state.ParenthesisLevel, _state.Operands.Count),
                in substring
                );
        }

        private void InvokeHigherPrecedenceOperators(in Operator @operator)
        {
            while (TryPeek(out Operator lastOperator))
            {
                if (lastOperator.IsOpenParenthesis)
                    break;
                
                if (lastOperator.Precedence > @operator.Precedence)
                    break;
                
                InvokeAfterDelayedOperators(Pop());
            }
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
