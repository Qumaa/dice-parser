namespace DiceRoll
{
    public abstract class Numeric : INumeric
    {
        private RollProbabilityDistribution _cachedDistribution;

        public Outcome Evaluation { get; private set; }

        public abstract void Next();

        public RollProbabilityDistribution GetProbabilityDistribution() =>
            _cachedDistribution ??= CreateProbabilityDistribution();

        public void Visit<T>(T visitor) where T : INodeVisitor =>
            visitor.ForNumeric(this);

        protected void CacheEvaluation(in Outcome evaluation) =>
            Evaluation = evaluation;

        protected abstract RollProbabilityDistribution CreateProbabilityDistribution();
    }
}
