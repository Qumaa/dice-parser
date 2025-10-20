namespace DiceRoll
{
    public sealed class OperationAsAssertion : Assertion
    {
        private readonly IOperation _operation;
        
        public OperationAsAssertion(IOperation operation) 
        {
            _operation = operation;
        }

        public override void NextEvaluation()
        {
            _operation.NextEvaluation();
            CacheEvaluation(_operation.CachedEvaluation.AsBinary());
        }
        
        protected override LogicalProbabilityDistribution CreateProbabilityDistribution() =>
            _operation.GetProbabilityDistribution().AsLogical();
    }
}
