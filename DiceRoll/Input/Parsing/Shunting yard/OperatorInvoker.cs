namespace DiceRoll.Input.Parsing
{
    public abstract class OperatorInvoker
    {
        // positive = straight
        // negative = takes parameters from the right; converted to positive by negating and adding 1
        private readonly int _arity;

        internal bool ExpectsRightOperands => _arity <= 0;

        internal int Arity => ExpectsRightOperands ? DecodeArityFromRightFlowDirection() : _arity;

        protected private OperatorInvoker(int arity, FlowDirection flowDirection = FlowDirection.Left)
        {
            _arity = flowDirection is FlowDirection.Left ?
                arity :
                EncodeRightFlowDirectionArity(arity);
        }

        public abstract void Invoke(OperandsStackAccess operands);

        private int DecodeArityFromRightFlowDirection() =>
            -_arity + 1;

        private static int EncodeRightFlowDirectionArity(int arity) =>
            -(arity - 1);

        protected enum FlowDirection
        {
            // ... 4 3 1 x 2
            Left = 0,
            // x 1 2 3 ...
            Right = 1
            // are (... 3 2 1 x) and (... 4 3 2 x 1) needed?
        }

        public static OperatorInvoker Binary<TLeft, TRight>(BinaryInvocationHandler<TLeft, TRight> handler)
            where TLeft : INode where TRight : INode =>
            new BinaryOperatorInvoker<TLeft, TRight>(handler);

        public static OperatorInvoker Unary<T>(UnaryInvocationHandler<T> handler) where T : INode =>
            new UnaryOperatorInvoker<T>(handler);
        
        public static OperatorInvoker ReversedUnary<T>(UnaryInvocationHandler<T> handler) where T : INode =>
            new ReversedUnaryOperatorInvoker<T>(handler);
    }
}
