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

        protected abstract RollProbabilityDistribution CreateProbabilityDistribution();

        protected abstract Outcome GetNextEvaluation();

        public void Visit(INodeVisitor visitor) =>
            visitor.ForNumeric(this);
    }
}
