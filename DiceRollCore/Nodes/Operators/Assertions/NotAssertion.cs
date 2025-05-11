namespace DiceRoll
{
    public sealed class NotAssertion : UnaryAssertion
    {
        public NotAssertion(IAssertion assertion) : base(assertion) { }

        protected override Binary GetNextEvaluation() =>
            !_assertion.Evaluation;

        protected override LogicalProbabilityDistribution CreateProbabilityDistribution() =>
            new(_assertion.GetProbabilityDistribution().False);
    }
}
