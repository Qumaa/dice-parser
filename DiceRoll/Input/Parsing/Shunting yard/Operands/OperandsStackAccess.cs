using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class OperandsStackAccess
    {
        private readonly OperandCastersTable _castersTable;
        private readonly MappedStack<LinkedNode> _operands;
        private readonly int _arity;
        private readonly int _popLimit;
        
        private readonly Mapped<LinkedNode>[] _operatorParents;
        private readonly Range _operatorRange;

        private Range _resultRange;

        private int _operatorParentsCount => _arity - (_operands.Count - _popLimit);

        public OperandsStackAccess(MappedStack<LinkedNode> operands, OperandCastersTable castersTable, int arity,
            in Range operatorRange)
        {
            _operands = operands;
            _castersTable = castersTable;
            _arity = arity;
            _operatorRange = _resultRange = operatorRange;
            
            _popLimit = operands.Count - arity;
            _operatorParents = new Mapped<LinkedNode>[arity];
        }

        public T Pop<T>() where T : INode
        {
            ThrowIfExceedingArity();

            int parentIndex = _operatorParentsCount;
            Mapped<LinkedNode> operand = _operands.Pop();
            _operatorParents[parentIndex] = operand;

            _resultRange = operand.Merge(_resultRange);
            
            return CastOrThrow<T>(operand.Value.Node);
        }

        internal void PushResult(INode operand)
        {
            ArgumentNullException.ThrowIfNull(operand);
            ThrowIfPushingPrematurely();

            LinkedNode linkedOperator = new(null, _operatorParents);
            Mapped<LinkedNode> mappedOperator = new(linkedOperator, in _operatorRange);

            LinkedNode linkedOperand = new(operand, mappedOperator);
            
            _operands.Push(linkedOperand, in _resultRange);
        }

        internal void Reset()
        {
            _resultRange = _operatorRange;

            for (int i = _operatorParentsCount - 1; i >= 0; i--)
                    _operands.Push(_operatorParents[i]);
        }

        private void ThrowIfExceedingArity()
        {
            if (_operands.Count <= _popLimit)
                throw new OperatorInvocationException(ParsingErrorMessages.ExceedingArity(_arity));
        }

        private void ThrowIfPushingPrematurely()
        {
            if (_operands.Count != _popLimit)
                throw new OperatorInvocationException(ParsingErrorMessages.PUSHING_PREMATURELY);
        }

        private T CastOrThrow<T>(INode node) where T : INode =>
            node is T operand || _castersTable.TryCast(node, out operand) ?
                operand :
                throw new OperatorInvocationException(
                    ParsingErrorMessages.OperandTypeMismatch(typeof(T), node.GetType())
                    );
    }
}
