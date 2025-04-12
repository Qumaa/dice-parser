using System.Runtime.InteropServices;

namespace DiceRoll.Input
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct OperandsStackAccess
    {
        private readonly FormulaSubstringsStack<INode> _operands;
        private readonly int _arity;
        private readonly int _popLimit;

        public OperandsStackAccess(FormulaSubstringsStack<INode> operands, int arity)
        {
            _operands = operands;
            _arity = arity;
            _popLimit = operands.Count - arity;
        }

        public T Pop<T>() where T : INode
        {
            ThrowIfExceedingArity();

            return CastOrThrow<T>(_operands.PopValue());
        }

        public void PushResult(INode operand)
        {
            ThrowIfPushingPrematurely();

            _operands.PushWithoutContext(operand);
        }

        private void ThrowIfExceedingArity()
        {
            if (_operands.Count - 1 < _popLimit)
                throw new OperatorInvocationException(ExceedingArityMessage());
        }

        private string ExceedingArityMessage()
        {
            const string message_format =
                "An attempt to work on more operands than the arity of the operator was intercepted. The operator's arity ({0}) is faulty.";
            
            return string.Format(message_format, _arity.ToString());
        }

        private void ThrowIfPushingPrematurely()
        {
            const string message = 
                "An attempt to return the result of working on fewer operands than the declared arity was intercepted.";
            
            if (_operands.Count != _popLimit)
                throw new OperatorInvocationException(message);
        }

        private static T CastOrThrow<T>(INode node) where T : INode =>
            node is T operand ?
                operand :
                throw new OperatorInvocationException(typeof(T), node.GetType());
    }
}
