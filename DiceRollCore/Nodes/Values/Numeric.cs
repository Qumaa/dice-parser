namespace DiceRoll
{
    public abstract class Numeric : INumeric
    {
        private RollProbabilityDistribution _cachedDistribution;

        public abstract Outcome Evaluate();

        public RollProbabilityDistribution GetProbabilityDistribution() =>
            _cachedDistribution ??= CreateProbabilityDistribution();

        protected abstract RollProbabilityDistribution CreateProbabilityDistribution();

        public void Visit(INodeVisitor visitor) =>
            visitor.ForNumeric(this);
    }
}
