namespace DiceRoll.Input
{
    public abstract class OperatorInvoker
    {
        // positive = straight
        // negative = takes parameters from the right; converted to positive by negating and adding 1
        private readonly int _arity;

        public bool ExpectsRightOperands => _arity <= 0;

        public int Arity => ExpectsRightOperands ? DecodeArityFromRightFlowDirection() : _arity;

        protected OperatorInvoker(int arity, FlowDirection flowDirection = FlowDirection.Left)
        {
            _arity = flowDirection is FlowDirection.Left ?
                arity :
                EncodeRightFlowDirectionArity(arity);
        }

        public abstract void Invoke(in OperandsStackAccess operands);

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
    }
}
