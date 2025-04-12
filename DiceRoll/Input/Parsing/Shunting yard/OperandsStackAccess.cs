using System.Runtime.InteropServices;

namespace DiceRoll.Input
{
    [StructLayout(LayoutKind.Auto)]
    public struct OperandsStackAccess
    {
        private const string _EXCESSIVE_POPPING_MESSAGE_FORMAT =
            "An attempt to work on more operands than the arity of the operator was intercepted. The operator's arity ({0}) is faulty.";
        private const string _PREMATURE_PUSH_MESSAGE = 
            "An attempt to return the result of working on fewer operands than the declared arity was intercepted.";

        private readonly FormulaSubstringsStack<INode> _operands;
        private readonly int _arity;
        private readonly int _popLimit;

        private int _start;
        private int _length;

        public OperandsStackAccess(FormulaSubstringsStack<INode> operands, int arity)
        {
            _operands = operands;
            _arity = arity;
            _popLimit = operands.Count - arity;

            FormulaSubstring<INode> peek = _operands.Peek();
            _start = peek.Range.Start.Value;
            _length = peek.Range.End.Value - _start;
        }

        public T Pop<T>() where T : INode
        {
            if (_operands.Count - 1 < _popLimit)
                throw new OperatorInvocationException(ExcessivePoppingMessage());

            FormulaSubstring<INode> context = _operands.Pop();

            int startDiff = _start - context.Range.Start.Value;
            _start -= startDiff;
            _length += startDiff;
                
            INode node = context.Value;
                
            return node is T operand ?
                operand :
                throw new OperatorInvocationException(typeof(T), node.GetType());
        }

        private string ExcessivePoppingMessage() =>
            string.Format(_EXCESSIVE_POPPING_MESSAGE_FORMAT, _arity.ToString());

        public void Push(INode operand)
        {
            if (_operands.Count != _popLimit)
                throw new OperatorInvocationException(_PREMATURE_PUSH_MESSAGE);
                
            _operands.Push(operand, _start, _length);
        }
    }
}
