namespace DiceRoll
{
    public sealed class BinaryConstant : Assertion
    {
        public BinaryConstant(bool value)
        {
            CacheEvaluation(new Binary(value));
        }

        public override void Next() { }

        protected override LogicalProbabilityDistribution CreateProbabilityDistribution() =>
            new(Evaluation ? Probability.Hundred : Probability.Zero);
    }
}
