namespace DiceRoll.Input.Parsing
{
    internal sealed class UnaryOperatorInvoker<TReturn, T> : OperatorInvoker where TReturn : INode where T : INode
    {
        private readonly UnaryInvocationHandler<TReturn, T> _handler;

        private UnaryOperatorInvoker(UnaryInvocationHandler<TReturn, T> handler, Arity arity) 
            : base(Signature.Arguments<T>().Returns<TReturn>(), arity)
        {
            _handler = handler;
        }

        public override INode Invoke(OperandsAccess operandsAccess)
        {
            operandsAccess.Get(0, out T node);
            
            return _handler.Invoke(node);
        }

        public static UnaryOperatorInvoker<TReturn, T> Prefix(UnaryInvocationHandler<TReturn, T> handler) =>
            new(handler, Arity.PrefixUnary);

        public static UnaryOperatorInvoker<TReturn, T> Postfix(UnaryInvocationHandler<TReturn, T> handler) =>
            new(handler, Arity.PostfixUnary);
    }
}
