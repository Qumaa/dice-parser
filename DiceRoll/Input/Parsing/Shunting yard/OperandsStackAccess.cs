using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input
{
    [StructLayout(LayoutKind.Auto)]
    public struct OperandsStackAccess
    {
        private readonly FormulaTokensStack<INode> _operands;
        private readonly int _arity;
        private readonly int _popLimit;
        
        private int _resultStart;
        private int _resultLength;

        public OperandsStackAccess(FormulaTokensStack<INode> operands, int arity)
        {
            _operands = operands;
            _arity = arity;
            _popLimit = operands.Count - arity;
            
            FormulaToken<INode> peek = _operands.Peek();
            _resultStart = peek.Range.Start.Value;
            _resultLength = peek.Range.End.Value - _resultStart;
        }

        public T Pop<T>() where T : INode
        {
            ThrowIfExceedingArity();

            FormulaToken<INode> operand = _operands.Pop();

            IncludeToOutputTokenRange(in operand.Range);

            return CastOrThrow<T>(operand.Value);
        }

        private void IncludeToOutputTokenRange(in Range range)
        {
            int startDiff = _resultStart - range.Start.Value;
            _resultStart -= startDiff;
            _resultLength += startDiff;
        }

        public void PushResult(INode operand)
        {
            ThrowIfPushingPrematurely();

            _operands.Push(operand, _resultStart, _resultLength);
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
