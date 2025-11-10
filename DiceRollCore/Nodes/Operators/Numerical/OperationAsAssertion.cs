namespace DiceRoll
{
    public class OperationAsAssertion : IAssertion
    {
        protected readonly IOperation _operation;
        private LogicalProbabilityDistribution _cachedDistribution;

        public Binary CachedEvaluation => _operation.CachedEvaluation.AsBinary();

        public Probability True => GetProbabilityDistribution().True;

        public OperationAsAssertion(IOperation operation) 
        {
            _operation = operation;
        }

        public void Visit<T>(T visitor) where T : INodeVisitor =>
            visitor.ForAssertion(this);

        public void NextEvaluation() =>
            _operation.NextEvaluation();

        public object Clone() =>
            new OperationAsAssertion(_operation.CloneTyped());

        public LogicalProbabilityDistribution GetProbabilityDistribution() =>
            _cachedDistribution ??= CreateProbabilityDistribution();
        
        protected virtual LogicalProbabilityDistribution CreateProbabilityDistribution() =>
            _operation.GetProbabilityDistribution().AsLogical();
        
    }
}
