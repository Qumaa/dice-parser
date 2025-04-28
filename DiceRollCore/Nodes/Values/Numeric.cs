namespace DiceRoll
{
    public abstract class Numeric : INumeric
    {
        private RollProbabilityDistribution _cachedDistribution;

        public Outcome Evaluation { get; private set; }

        public void Next() =>
            Evaluation = GetNextEvaluation();

        public RollProbabilityDistribution GetProbabilityDistribution() =>
            _cachedDistribution ??= CreateProbabilityDistribution();

        public void Visit<T>(T visitor) where T : INodeVisitor =>
            visitor.ForNumeric(this);

        protected abstract RollProbabilityDistribution CreateProbabilityDistribution();

        protected abstract Outcome GetNextEvaluation();
    }
}
