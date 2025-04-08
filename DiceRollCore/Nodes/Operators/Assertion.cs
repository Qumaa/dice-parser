namespace DiceRoll
{
    public abstract class Assertion : IAssertion
    {
        private LogicalProbabilityDistribution _cachedDist;

        public Probability True => GetProbabilityDistribution().True;
        public abstract Binary Evaluate();
        public LogicalProbabilityDistribution GetProbabilityDistribution() =>
            _cachedDist ??= CreateProbabilityDistribution();
        public void Visit(INodeVisitor visitor) =>
            visitor.ForAssertion(this);

        protected abstract LogicalProbabilityDistribution CreateProbabilityDistribution();
    }
}
