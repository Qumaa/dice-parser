using System;

namespace DiceRoll.Input.Parsing
{
    public abstract class OperatorInvoker
    {
        public readonly Signature Signature;
        
        public readonly int LeftArity;
        public readonly int RightArity;
        
        public int Arity => LeftArity + RightArity;
        
        protected OperatorInvoker(Signature signature, int leftArity, int rightArity)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(leftArity);
            ArgumentOutOfRangeException.ThrowIfNegative(rightArity);
            ArgumentOutOfRangeException.ThrowIfZero(leftArity + rightArity);
            ArgumentOutOfRangeException.ThrowIfNotEqual(signature.OperandsNumber, leftArity + rightArity);
            
            Signature = signature;
            LeftArity = leftArity;
            RightArity = rightArity;
        }

        public abstract INode Invoke(OperandsAccess operandsAccess);
        
        public static OperatorInvoker Binary<TReturn, TLeft, TRight>(BinaryInvocationHandler<TReturn, TLeft, TRight> handler)
            where TReturn : INode where TLeft : INode where TRight : INode =>
            new BinaryOperatorInvoker<TReturn, TLeft, TRight>(handler);

        public static OperatorInvoker PrefixUnary<TReturn, T>(UnaryInvocationHandler<TReturn, T> handler) 
            where TReturn : INode where T : INode =>
            UnaryOperatorInvoker<TReturn, T>.Prefix(handler);

        public static OperatorInvoker PostfixUnary<TReturn, T>(UnaryInvocationHandler<TReturn, T> handler) 
            where TReturn : INode where T : INode =>
            UnaryOperatorInvoker<TReturn, T>.Postfix(handler);

        public static OperatorInvoker Composition(CompositionHandler handler) =>
            new CompositionInvoker(handler);
    }
}
