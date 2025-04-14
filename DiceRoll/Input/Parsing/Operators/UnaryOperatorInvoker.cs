namespace DiceRoll.Input
{
    internal sealed class UnaryOperatorInvoker<T> : OperatorInvoker where T : INode
    {
        private readonly UnaryInvocationHandler<T> _handler;
        
        public UnaryOperatorInvoker(UnaryInvocationHandler<T> handler) : base(1, FlowDirection.Right)
        {
            _handler = handler;
        }

        public override void Invoke(OperandsStackAccess operands) =>
            operands.PushResult(_handler.Invoke(operands.Pop<T>()));
    }
}
