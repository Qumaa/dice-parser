namespace DiceRoll
{
    public sealed class NotAssertion : UnaryAssertion
    {
        public NotAssertion(IAssertion source) : base(source) { }

        public override void NextEvaluation()
        {
            base.NextEvaluation();
            CacheEvaluation(!_source.CachedEvaluation);
        }
        
        protected override LogicalProbabilityDistribution CreateProbabilityDistribution() =>
            new(_source.GetProbabilityDistribution().False);
    }
}
