namespace DiceRoll
{
    public sealed class OperationAsAssertion : Assertion
    {
        private readonly IOperation _operation;
        
        public OperationAsAssertion(IOperation operation) 
        {
            _operation = operation;
        }

        public override Binary Evaluate() =>
            _operation.Evaluate().AsBinary();

        protected override LogicalProbabilityDistribution CreateProbabilityDistribution() =>
            _operation.GetProbabilityDistribution().AsLogical();
    }
}
