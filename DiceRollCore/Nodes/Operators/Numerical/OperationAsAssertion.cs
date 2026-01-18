namespace DiceRoll
{
    public class OperationAsAssertion : IAssertion
    {
        public readonly IOperation Source;
        private LogicalProbabilityDistribution _cachedDistribution;

        public Binary CachedEvaluation => Source.CachedEvaluation.AsBinary();

        public Probability True => GetProbabilityDistribution().True;

        public OperationAsAssertion(IOperation source) 
        {
            Source = source;
        }

        public virtual void Visit<T>(T visitor) where T : INodeVisitor =>
            visitor.ForAssertion(this);

        public void NextEvaluation() =>
            Source.NextEvaluation();

        public object Clone() =>
            new OperationAsAssertion(Source.CloneTyped());

        public LogicalProbabilityDistribution GetProbabilityDistribution() =>
            _cachedDistribution ??= CreateProbabilityDistribution();
        
        protected virtual LogicalProbabilityDistribution CreateProbabilityDistribution() =>
            Source.GetProbabilityDistribution().AsLogical();
        
    }
}
