namespace DiceRoll
{
    public abstract class Assertion : IAssertion
    {
        private LogicalProbabilityDistribution _cachedDistribution;

        public Probability True => GetProbabilityDistribution().True;
        public Binary CachedEvaluation { get; private set; }

        public abstract void NextEvaluation();

        public LogicalProbabilityDistribution GetProbabilityDistribution() =>
            _cachedDistribution ??= CreateProbabilityDistribution();

        public virtual void Visit<T>(T visitor) where T : INodeVisitor =>
            visitor.ForAssertion(this);

        public virtual object Clone() =>
            MemberwiseClone();

        protected void CacheEvaluation(in Binary evaluation) =>
            CachedEvaluation = evaluation;

        protected abstract LogicalProbabilityDistribution CreateProbabilityDistribution();
    }
}
