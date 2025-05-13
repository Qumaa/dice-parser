namespace DiceRoll
{
    public sealed class OperationAsAssertion : Assertion
    {
        private readonly IOperation _operation;
        
        public OperationAsAssertion(IOperation operation) 
        {
            _operation = operation;
        }

        public override void Next()
        {
            _operation.Next();
            CacheEvaluation(_operation.Evaluation.AsBinary());
        }

        protected override LogicalProbabilityDistribution CreateProbabilityDistribution() =>
            _operation.GetProbabilityDistribution().AsLogical();
    }
}
