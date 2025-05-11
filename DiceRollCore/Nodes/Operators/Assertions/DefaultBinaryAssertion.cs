namespace DiceRoll
{
    public sealed class DefaultBinaryAssertion : BinaryAssertion
    {
        private readonly BinaryOperationDelegates _delegates;

        public DefaultBinaryAssertion(IAssertion left, IAssertion right, BinaryAssertionType assertionType) :
            base(left, right)
        {
            _delegates = DefaultAssertionDelegates.Get(assertionType);
        }

        protected override Binary GetNextEvaluation() =>
            _delegates.Evaluation.Invoke(_left.Evaluation, _right.Evaluation);

        protected override LogicalProbabilityDistribution CreateProbabilityDistribution() =>
            new(_delegates.Probability.Invoke(_left.GetProbabilityDistribution(), _right.GetProbabilityDistribution()));
    }
}
