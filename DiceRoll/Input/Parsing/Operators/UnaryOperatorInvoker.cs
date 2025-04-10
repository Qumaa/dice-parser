namespace DiceRoll.Input
{
    public sealed class UnaryOperatorInvoker<T> : ShuntingYard.OperatorInvoker where T : INode
    {
        private readonly ReversedUnaryOperatorInvoker<T> _delayedInvoker;
        
        public UnaryOperatorInvoker(UnaryInvocationHandler<T> handler) : base(0)
        {
            _delayedInvoker = new ReversedUnaryOperatorInvoker<T>(handler);
        }

        public override void Invoke(ShuntingYard.OperandsStackAccess operands) =>
            operands.ForNextOperand(_delayedInvoker);
    }
}
