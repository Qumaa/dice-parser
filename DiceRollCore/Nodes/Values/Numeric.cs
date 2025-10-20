namespace DiceRoll
{
    public abstract class Numeric : INumeric
    {
        private RollProbabilityDistribution _cachedDistribution;

        public Outcome CachedEvaluation { get; private set; }

        public abstract void NextEvaluation();

        public RollProbabilityDistribution GetProbabilityDistribution() =>
            _cachedDistribution ??= CreateProbabilityDistribution();

        public void Visit<T>(T visitor) where T : INodeVisitor =>
            visitor.ForNumeric(this);

        public virtual object Clone() =>
            MemberwiseClone();

        protected void CacheEvaluation(in Outcome evaluation) =>
            CachedEvaluation = evaluation;

        protected abstract RollProbabilityDistribution CreateProbabilityDistribution();
    }
}
