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
            NodeTree node = CollapseOperatorsStack();
            _state.Mapper.Clear();
            _state.Annotate().NewExpressionStart();
            return node;
        }
        
        private NodeTree CollapseOperatorsStack()
        {
            ThrowIfAnyTrailingOperators();

            while(_state.Operators.TryPop(out Mapped<Operator> context))
                _state.InvocationHandler.InvokeOperator(in context);

            Mapped<LinkedNode> result = _state.Operands.Pop();
            SubstringMapper mapper = _state.Mapper.BuildSubstringMapper();
            
            ThrowIfAnyOperandLeft();

            return new NodeTree(mapper, result);
        }
        
        private void ThrowIfAnyTrailingOperators()
        {
            if (!_state.DelayedOperators.TryPeek(out Mapped<DelayedOperator> delayedOperator))
                return;

            int received = _state.Operands.Count - delayedOperator.Value.CapturedOperands;
            int expected = delayedOperator.Value.InvocationBehaviour.RightArity;

            string message =
                $"This operator didn't receive enough right-side operands. Expected {expected}, but received {received}.";
            
            MapAndThrow(in delayedOperator.Range, message);
        }
        
        private void ThrowIfAnyOperandLeft()
        {
            if (_state.Operands.TryPeek(out Mapped<LinkedNode> operandToken))
                MapAndThrow(in operandToken.Range, "This operand doesn't take part in the expression.");
        }

        private void MapAndThrow(in Range exceptionCause, string message)
        {
            throw new ParsingException(_state.Mapper.GetSubstringOf(in exceptionCause), message);
        }
    }
}
