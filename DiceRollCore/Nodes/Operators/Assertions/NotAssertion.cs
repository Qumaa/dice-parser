namespace DiceRoll
{
    public sealed class NotAssertion : UnaryAssertion
    {
        public NotAssertion(IAssertion source) : base(source) { }

        public override void NextEvaluation()
        {
            base.NextEvaluation();
            CacheEvaluation(!Source.CachedEvaluation);
        }
        
        protected override LogicalProbabilityDistribution CreateProbabilityDistribution() =>
            new(Source.GetProbabilityDistribution().False);
    }
}
