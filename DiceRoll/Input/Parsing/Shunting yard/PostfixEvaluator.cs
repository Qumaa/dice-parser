namespace DiceRoll.Input
{
    internal sealed class PostfixEvaluator
    {
        private readonly ShuntingYardState _state;
        private readonly ShuntingYardOperators _operators;
        private readonly ShuntingYardOperands _operands;
        
        public PostfixEvaluator(ShuntingYardState state, ShuntingYardOperators operators, ShuntingYardOperands operands)
        {
            _state = state;
            _operators = operators;
            _operands = operands;
        }

        public INode Evaluate()
        {
            INode node = CollapseOperatorsStack();
            _state.Accumulator.Clear();
            _state.DenoteNewExpressionStart();
            return node;
        }
        
        private INode CollapseOperatorsStack()
        {
            ThrowIfAnyTrailingOperators();

            while(_operators.TryPop(out FormulaToken<OperatorToken> context))
                _operators.InvokeOperatorOrThrow(in context);

            INode result = _operands.Pop();
            
            ThrowIfAnyOperandLeft();

            return result;
        }
        
        private void ThrowIfAnyTrailingOperators()
        {
            if (_operators.TryPeek(out FormulaToken<DelayedOperatorToken> operatorToken))
                _state.Throw(in operatorToken, ParsingErrorMessages.TRAILING_DELAYED_OPERATOR);
        }
        
        private void ThrowIfAnyOperandLeft()
        {
            if (_operands.TryPeek(out FormulaToken<INode> operandToken))
                _state.Throw(in operandToken, ParsingErrorMessages.UNUSED_OPERAND);
        }
    }
}
