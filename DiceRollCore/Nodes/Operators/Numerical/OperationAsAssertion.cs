namespace DiceRoll
{
    public sealed class OperationAsAssertion : Assertion
    {
        private readonly IOperation _operation;
        
        public OperationAsAssertion(IOperation operation) 
        {
            _operation = operation;
        }

        protected override Binary GetNextEvaluation() =>
            _operation.Evaluation.AsBinary();

        protected override LogicalProbabilityDistribution CreateProbabilityDistribution() =>
            _operation.GetProbabilityDistribution().AsLogical();
    }
}
