namespace DiceRoll
{
    public sealed class DefaultNumericOperation : NumericOperation
    {
        private readonly NumericOperationDelegates _delegates;

        public DefaultNumericOperation(IAnalyzable left, IAnalyzable right, NumericOperationType operationType) : base(left, right)
        {
            _delegates = DefaultNumericOperationDelegates.Get(operationType);
        }

        public override LogicalProbabilityDistribution GetProbabilityDistribution() =>
            new(_delegates.Probability.Invoke(_left.GetProbabilityDistribution(), _right.GetProbabilityDistribution()));

        public override Binary Evaluate() =>
            _delegates.Evaluation(_left.Evaluate(), _right.Evaluate());
    }
}
