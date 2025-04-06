namespace DiceRoll.Input
{
    public sealed class UnaryOperatorInvoker<T> : RPNOperatorInvoker where T : INode
    {
        private readonly ReversedUnaryOperatorInvoker<T> _delayedInvoker;
        
        public UnaryOperatorInvoker(UnaryInvocationHandler<T> handler) : base(0)
        {
            _delayedInvoker = new ReversedUnaryOperatorInvoker<T>(handler);
        }

        public override void Invoke(DiceExpressionParser.OperandsStackAccess operands) =>
            operands.ForNextOperand(_delayedInvoker);
    }
}
