namespace DiceRoll.Input.Parsing
{
    public abstract class OperatorInvoker
    {
        public readonly ArgumentsLayout Layout;
        public readonly int Arity;

        protected OperatorInvoker(int arity, ArgumentsLayout layout = ArgumentsLayout.Left)
        {
            Arity = arity;
            Layout = layout;
        }

        public abstract void Invoke(OperandsStackAccess operands);
        
        public static OperatorInvoker Binary<TLeft, TRight>(BinaryInvocationHandler<TLeft, TRight> handler)
            where TLeft : INode where TRight : INode =>
            new BinaryOperatorInvoker<TLeft, TRight>(handler);

        public static OperatorInvoker Unary<T>(UnaryInvocationHandler<T> handler) where T : INode =>
            new UnaryOperatorInvoker<T>(handler);
        
        public static OperatorInvoker ReversedUnary<T>(UnaryInvocationHandler<T> handler) where T : INode =>
            new ReversedUnaryOperatorInvoker<T>(handler);
    }
}
