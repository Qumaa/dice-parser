namespace DiceRoll.Input.Parsing
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

        public NodeTree Evaluate()
        {
            NodeTree node = CollapseOperatorsStack();
            _state.Mapper.Clear();
            _state.Annotate().NewExpressionStart();
            return node;
        }
        
        private NodeTree CollapseOperatorsStack()
        {
            ThrowIfAnyTrailingOperators();

            while(_operators.TryPop(out Mapped<Operator> context))
                _operators.InvokeOperatorOrThrow(in context);

            Mapped<LinkedNode> result = _operands.Pop();
            SubstringMapper mapper = _state.Mapper.BuildSubstringSource();
            
            ThrowIfAnyOperandLeft();

            return new NodeTree(mapper, result);
        }
        
        private void ThrowIfAnyTrailingOperators()
        {
            if (_operators.TryPeek(out Mapped<DelayedOperator> operatorToken))
                _state.MapAndThrow(in operatorToken, ParsingErrorMessages.TRAILING_DELAYED_OPERATOR);
        }
        
        private void ThrowIfAnyOperandLeft()
        {
            if (_operands.TryPeek(out Mapped<LinkedNode> operandToken))
                _state.MapAndThrow(in operandToken, ParsingErrorMessages.UNUSED_OPERAND);
        }
    }
}
