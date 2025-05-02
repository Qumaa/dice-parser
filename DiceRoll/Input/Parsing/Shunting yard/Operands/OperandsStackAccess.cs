using System;
using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public sealed class OperandsStackAccess
    {
        private readonly MappedStack<LinkedNode> _operands;
        private readonly int _arity;
        private readonly int _popLimit;
        
        private readonly List<Mapped<LinkedNode>> _operatorParents;
        private readonly Range _operatorRange;

        private Range _resultRange;

        public OperandsStackAccess(MappedStack<LinkedNode> operands, int arity, in Range operatorRange)
        {
            _operands = operands;
            _arity = arity;
            _operatorRange = operatorRange;
            _popLimit = operands.Count - arity;
            _operatorParents = new List<Mapped<LinkedNode>>(arity);

            _resultRange = operatorRange;
        }

        public T Pop<T>() where T : INode
        {
            ThrowIfExceedingArity();

            Mapped<LinkedNode> operand = _operands.Pop();
            _operatorParents.Add(operand);

            _resultRange = operand.Merge(_resultRange);
            
            return CastOrThrow<T>(operand.Value.Node);
        }

        public void PushResult(INode operand)
        {
            ArgumentNullException.ThrowIfNull(operand);
            ThrowIfPushingPrematurely();

            LinkedNode linkedOperator = new(null, _operatorParents.ToArray());
            Mapped<LinkedNode> mappedOperator = new(linkedOperator, in _operatorRange);

            LinkedNode linkedOperand = new(operand, mappedOperator);
            
            _operands.Push(linkedOperand, in _resultRange);
        }

        private void ThrowIfExceedingArity()
        {
            if (_operands.Count - 1 < _popLimit)
                throw new OperatorInvocationException(ParsingErrorMessages.ExceedingArity(_arity));
        }

        private void ThrowIfPushingPrematurely()
        {
            if (_operands.Count != _popLimit)
                throw new OperatorInvocationException(ParsingErrorMessages.PUSHING_PREMATURELY);
        }

        private static T CastOrThrow<T>(INode node) where T : INode =>
            node is T operand ?
                operand :
                throw new OperatorInvocationException(
                    ParsingErrorMessages.OperandTypeMismatch(typeof(T), node.GetType())
                    );
    }
}
