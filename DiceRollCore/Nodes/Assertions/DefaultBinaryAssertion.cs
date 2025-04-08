namespace DiceRoll
{
    public sealed class DefaultBinaryAssertion : BinaryAssertion
    {
        private readonly BinaryOperationDelegates _delegates;

        public DefaultBinaryAssertion(IAssertion left, IAssertion right, BinaryAssertionType assertionType) : base(left, right)
        {
            _delegates = BinaryAssertionDelegates.Get(assertionType);
        }

        public override Binary Evaluate() =>
            _delegates.Evaluation.Invoke(_left.Evaluate(), _right.Evaluate());

        protected override LogicalProbabilityDistribution CreateProbabilityDistribution() =>
            new(_delegates.Probability.Invoke(_left.GetProbabilityDistribution(), _right.GetProbabilityDistribution()));
    }
}
