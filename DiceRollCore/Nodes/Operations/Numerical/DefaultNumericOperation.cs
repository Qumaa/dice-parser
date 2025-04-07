namespace DiceRoll
{
    public sealed class DefaultNumericOperation : NumericOperation
    {
        private readonly NumericOperationDelegates _delegates;

        public DefaultNumericOperation(INumeric left, INumeric right, NumericOperationType operationType) : base(left, right)
        {
            _delegates = DefaultNumericOperationDelegates.Get(operationType);
        }

        public override LogicalProbabilityDistribution GetProbabilityDistribution() =>
            new(_delegates.Probability(_left.GetProbabilityDistribution(), _right.GetProbabilityDistribution()));

        public override OptionalRollProbabilityDistribution GetOptionalRollsProbabilityDistribution() =>
            _delegates.OptionalRolls(_left.GetProbabilityDistribution(), _right.GetProbabilityDistribution());

        public override Binary Evaluate() =>
            _delegates.Evaluation(_left.Evaluate(), _right.Evaluate());
    }
}
