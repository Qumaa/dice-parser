namespace DiceRoll
{
    public sealed class NotAssertion : UnaryAssertion
    {
        public NotAssertion(IAssertion source) : base(source) { }
        
        public override void Next()
        {
            base.Next();
            CacheEvaluation(!_source.Evaluation);
        }

        protected override LogicalProbabilityDistribution CreateProbabilityDistribution() =>
            new(_source.GetProbabilityDistribution().False);
    }
}
