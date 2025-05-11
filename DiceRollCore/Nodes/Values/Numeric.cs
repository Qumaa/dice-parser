namespace DiceRoll
{
    public abstract class Numeric : INumeric
    {
        private RollProbabilityDistribution _cachedDistribution;

        public abstract Outcome Evaluation { get; }

        public abstract void Next();

        public RollProbabilityDistribution GetProbabilityDistribution() =>
            _cachedDistribution ??= CreateProbabilityDistribution();

        public void Visit<T>(T visitor) where T : INodeVisitor =>
            visitor.ForNumeric(this);

        protected abstract RollProbabilityDistribution CreateProbabilityDistribution();
    }
}
