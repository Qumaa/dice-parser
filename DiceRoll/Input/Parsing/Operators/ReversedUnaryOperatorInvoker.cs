namespace DiceRoll.Input
{
    public sealed class ReversedUnaryOperatorInvoker<T> : OperatorInvoker where T : INode
    {
        private readonly UnaryInvocationHandler<T> _handler;
        
        public ReversedUnaryOperatorInvoker(UnaryInvocationHandler<T> handler) : base(1)
        {
            _handler = handler;
        }

        public override void Invoke(OperandsStackAccess operands) =>
            operands.PushResult(_handler.Invoke(operands.Pop<T>()));
    }
}
