namespace DiceRoll.Input.Parsing
{
    internal sealed class RightUnaryOperatorInvoker<T> : OperatorInvoker where T : INode
    {
        private readonly UnaryInvocationHandler<T> _handler;
        
        public RightUnaryOperatorInvoker(UnaryInvocationHandler<T> handler) : base(0, 1)
        {
            _handler = handler;
        }

        public override INode Invoke(OperandsStackAccess operands) =>
            _handler.Invoke(operands.Pop<T>());
    }
}
