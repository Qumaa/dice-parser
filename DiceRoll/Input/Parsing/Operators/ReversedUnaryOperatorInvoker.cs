namespace DiceRoll.Input
{
    public sealed class ReversedUnaryOperatorInvoker<T> : RPNOperatorInvoker where T : INode
    {
        private readonly UnaryInvocationHandler<T> _handler;
        
        public ReversedUnaryOperatorInvoker(UnaryInvocationHandler<T> handler) : base(1)
        {
            _handler = handler;
        }

        public override void Invoke(DiceExpressionParser.OperandsStackAccess operands) =>
            operands.Push(_handler.Invoke(operands.Pop<T>()));
    }
}
