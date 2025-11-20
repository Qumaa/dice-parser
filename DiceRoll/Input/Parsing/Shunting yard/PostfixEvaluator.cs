namespace DiceRoll.Input.Parsing.Deprecated
{
    internal sealed class PostfixEvaluator
    {
        private readonly ShuntingYardState _state;

        public PostfixEvaluator(ShuntingYardState state)
        {
            _state = state;
        }

        public NodeTree Evaluate()
        {
            NodeTree tree = CollapseOperatorsStack();
            
            _state.Mapper.Clear();
            _state.Annotate().ExpressionStart();
            
            return tree;
        }
        
        private NodeTree CollapseOperatorsStack()
        {
            while(_state.Operators.TryPop(out Mapped<Operator> @operator))
                _state.InvocationHandler.InvokeOperator(in @operator);

            Mapped<LinkedNode> result = GetResult();
            SubstringMapper mapper = _state.Mapper.BuildSubstringMapper();

            return new NodeTree(mapper, result);
        }

        private Mapped<LinkedNode> GetResult()
        {
            if (_state.Operands.Count is 1)
                return _state.Operands.Pop();

            return NodePoolUtils.GroupNodes(_state.Operands.PopAll());
        }
    }
}
