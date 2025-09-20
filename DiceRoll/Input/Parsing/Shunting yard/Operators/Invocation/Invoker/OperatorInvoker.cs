namespace DiceRoll.Input.Parsing
{
    public abstract class OperatorInvoker
    {
        public readonly Signature Signature;
        
        protected OperatorInvoker(Signature signature)
        {
            Signature = signature;
        }

        public abstract INode Invoke(OperandsAccess operandsAccess);
        
        public static OperatorInvoker Binary<TReturn, TLeft, TRight>(BinaryInvocationHandler<TReturn, TLeft, TRight> handler)
            where TReturn : INode where TLeft : INode where TRight : INode =>
            new BinaryOperatorInvoker<TReturn, TLeft, TRight>(handler);

        public static OperatorInvoker Unary<TReturn, T>(UnaryInvocationHandler<TReturn, T> handler) 
            where TReturn : INode where T : INode =>
            new UnaryOperatorInvoker<TReturn, T>(handler);

        public static OperatorInvoker Composition(CompositionHandler handler) =>
            new CompositionInvoker(handler);
    }
}
