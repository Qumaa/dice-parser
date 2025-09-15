namespace DiceRoll.Input.Parsing
{
    internal sealed class PrefixUnaryOperatorInvoker<T> : OperatorInvoker where T : INode
    {
        private readonly UnaryInvocationHandler<T> _handler;
        
        public PrefixUnaryOperatorInvoker(UnaryInvocationHandler<T> handler) : base(0, 1)
        {
            _handler = handler;
        }

        public override INode Invoke(OperandsStackAccess operands) =>
            _handler.Invoke(operands.Pop<T>());
    }
}
