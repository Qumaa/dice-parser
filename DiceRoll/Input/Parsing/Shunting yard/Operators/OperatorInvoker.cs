namespace DiceRoll.Input.Parsing
{
    public abstract class OperatorInvoker
    {
        public readonly int LeftArity;
        public readonly int RightArity;

        public int Arity => LeftArity + RightArity;

        protected OperatorInvoker(int leftArity, int rightArity)
        {
            LeftArity = leftArity;
            RightArity = rightArity;
        }

        public abstract INode Invoke(OperandsStackAccess operands);
        
        public static OperatorInvoker Binary<TLeft, TRight>(BinaryInvocationHandler<TLeft, TRight> handler)
            where TLeft : INode where TRight : INode =>
            new BinaryOperatorInvoker<TLeft, TRight>(handler);

        public static OperatorInvoker PrefixUnary<T>(UnaryInvocationHandler<T> handler) where T : INode =>
            new PrefixUnaryOperatorInvoker<T>(handler);
        
        public static OperatorInvoker PostfixUnary<T>(UnaryInvocationHandler<T> handler) where T : INode =>
            new PostfixUnaryOperatorInvoker<T>(handler);
    }
}
