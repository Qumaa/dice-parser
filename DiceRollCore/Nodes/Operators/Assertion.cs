namespace DiceRoll
{
    public abstract class Assertion : IAssertion
    {
        private LogicalProbabilityDistribution _cachedDistribution;

        public Probability True => GetProbabilityDistribution().True;
        public abstract Binary Evaluate();
        public LogicalProbabilityDistribution GetProbabilityDistribution() =>
            _cachedDistribution ??= CreateProbabilityDistribution();
        public void Visit(INodeVisitor visitor) =>
            visitor.ForAssertion(this);

        protected abstract LogicalProbabilityDistribution CreateProbabilityDistribution();
    }
}
