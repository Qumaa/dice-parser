namespace DiceRoll
{
    public abstract class Assertion : IAssertion
    {
        private LogicalProbabilityDistribution _cachedDistribution;

        public Probability True => GetProbabilityDistribution().True;
        public Binary Evaluation { get; private set; }

        public abstract void Next();

        public LogicalProbabilityDistribution GetProbabilityDistribution() =>
            _cachedDistribution ??= CreateProbabilityDistribution();

        public void Visit<T>(T visitor) where T : INodeVisitor =>
            visitor.ForAssertion(this);

        protected void CacheEvaluation(in Binary evaluation) =>
            Evaluation = evaluation;
        
        protected abstract LogicalProbabilityDistribution CreateProbabilityDistribution();
    }
}
