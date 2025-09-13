namespace DiceRoll.Input.Parsing
{
    internal sealed class LeftUnaryOperatorInvoker<T> : OperatorInvoker where T : INode
    {
        private readonly UnaryInvocationHandler<T> _handler;
        
        public LeftUnaryOperatorInvoker(UnaryInvocationHandler<T> handler) : base(1, 0)
        {
            _handler = handler;
        }

        public override INode Invoke(OperandsStackAccess operands) =>
            _handler.Invoke(operands.Pop<T>());
    }
}
