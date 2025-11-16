using System;

namespace DiceRoll.Input.Parsing
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

            Mapped<LinkedNode> result = _state.Operands.Pop();
            SubstringMapper mapper = _state.Mapper.BuildSubstringMapper();
            
            ThrowIfAnyOperandLeft(mapper);

            return new NodeTree(mapper, result);
        }
        
        private void ThrowIfAnyOperandLeft(SubstringMapper mapper)
        {
            if (_state.Operands.TryPeek(out Mapped<LinkedNode> operandToken))
                throw new ParsingException(
                    mapper.GetSubstring(in operandToken.Range),
                    "This operand doesn't take part in the expression."
                    );
        }
    }
}
