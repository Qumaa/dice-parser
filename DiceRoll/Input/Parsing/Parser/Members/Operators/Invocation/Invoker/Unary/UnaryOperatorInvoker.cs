namespace DiceRoll.Input.Parsing
{
    internal sealed class UnaryOperatorInvoker<TReturn, T> : OperatorInvoker where TReturn : INode where T : INode
    {
        private readonly UnaryInvocationHandler<TReturn, T> _handler;

        private UnaryOperatorInvoker(UnaryInvocationHandler<TReturn, T> handler, int leftArity, int rightArity) : base(
            Signature.Arguments<T>().Returns<TReturn>(),
            leftArity,
            rightArity
            )
        {
            _handler = handler;
        }

        public override INode Invoke(OperandsAccess operandsAccess)
        {
            operandsAccess.Get(0, out T node);
            
            return _handler.Invoke(node);
        }

        public static UnaryOperatorInvoker<TReturn, T> Prefix(UnaryInvocationHandler<TReturn, T> handler) =>
            new(handler, 0, 1);

        public static UnaryOperatorInvoker<TReturn, T> Postfix(UnaryInvocationHandler<TReturn, T> handler) =>
            new(handler, 1, 0);
    }
}
