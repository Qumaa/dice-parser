using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    public struct OperandsStackAccess
    {
        private readonly MappedStack<INode> _operands;
        private readonly int _arity;
        private readonly int _popLimit;

        private Range _resultRange;

        public OperandsStackAccess(MappedStack<INode> operands, int arity)
        {
            _operands = operands;
            _arity = arity;
            _popLimit = operands.Count - arity;

            _resultRange = _operands.Peek().Range;
        }

        public T Pop<T>() where T : INode
        {
            ThrowIfExceedingArity();

            Mapped<INode> operand = _operands.Pop();

            _resultRange = operand.Merge(_resultRange);
            
            return CastOrThrow<T>(operand.Value);
        }

        public void PushResult(INode operand)
        {
            ArgumentNullException.ThrowIfNull(operand);
            ThrowIfPushingPrematurely();

            _operands.Push(operand, _resultRange);
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
