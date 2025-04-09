namespace DiceRoll
{
    public sealed class NotAssertion : UnaryAssertion
    {
        public NotAssertion(IAssertion assertion) : base(assertion) { }

        public override Binary Evaluate() =>
            !_assertion.Evaluate();

        protected override LogicalProbabilityDistribution CreateProbabilityDistribution() =>
            new(_assertion.GetProbabilityDistribution().False);
    }
}
