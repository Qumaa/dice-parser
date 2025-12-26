using System;

namespace DiceRoll.Input.Parsing
{
    public abstract class OperatorInvoker
    {
        public readonly Signature Signature;
        
        public readonly Arity Arity;

        protected OperatorInvoker(Signature signature, Arity arity)
        {
            ArgumentNullException.ThrowIfNull(signature);
            ArgumentOutOfRangeException.ThrowIfNotEqual(signature.OperandsNumber, arity.Total);

            Signature = signature;
            Arity = arity;
        }

        protected OperatorInvoker(Signature signature, int leftArity, int rightArity) : this(
            signature,
            new Arity(leftArity, rightArity)
            ) { }

        protected OperatorInvoker(Signature signature, int position) : this(
            signature,
            signature.DeriveArity(position)
            ) { }

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
