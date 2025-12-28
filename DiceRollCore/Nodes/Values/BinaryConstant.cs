namespace DiceRoll
{
    public sealed class BinaryConstant : Assertion
    {
        public bool Value => CachedEvaluation.Value;
        
        public BinaryConstant(bool value)
        {
            CacheEvaluation(new Binary(value));
        }

        public override void NextEvaluation() { }

        protected override LogicalProbabilityDistribution CreateProbabilityDistribution() =>
            new(CachedEvaluation ? Probability.Hundred : Probability.Zero);
    }
}
