namespace DiceRoll.Input
{
    public sealed class UnaryOperatorInvoker<T> : ShuntingYard.OperatorInvoker where T : INode
    {
        private readonly UnaryInvocationHandler<T> _handler;
        
        public UnaryOperatorInvoker(UnaryInvocationHandler<T> handler) : base(1, FlowDirection.Right)
        {
            _handler = handler;
        }

        public override void Invoke(in ShuntingYard.OperandsStackAccess operands) =>
            operands.Push(_handler.Invoke(operands.Pop<T>()));
    }
}
