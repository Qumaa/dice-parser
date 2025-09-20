namespace DiceRoll.Input.Parsing
{
    internal sealed class UnaryOperatorInvoker<TReturn, T> : OperatorInvoker where TReturn : INode where T : INode
    {
        private readonly UnaryInvocationHandler<TReturn, T> _handler;

        public UnaryOperatorInvoker(UnaryInvocationHandler<TReturn, T> handler) : base(
            Signature.Define<T>().Returns<TReturn>()
            )
        {
            _handler = handler;
        }

        public override INode Invoke(OperandsAccess operandsAccess)
        {
            operandsAccess.Get(0, out T node);
            
            return _handler.Invoke(node);
        }
    }
}
