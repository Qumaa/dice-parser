namespace DiceRoll
{
    public sealed class DefaultBinaryOperation : BinaryOperation
    {
        public readonly OperationType OperationType;
        private readonly OperationDelegates _delegates;

        public DefaultBinaryOperation(INumeric left, INumeric right, OperationType operationType) : base(left, right)
        {
            _delegates = DefaultOperationDelegates.Get(operationType);
            OperationType = operationType;
        }

        public override void NextEvaluation()
        {
            Outcome left = Left.Evaluate();
            Outcome right = Right.Evaluate();
            
            CacheEvaluation(_delegates.Evaluation(left, right));
        }

        protected override OptionalRollProbabilityDistribution CreateProbabilityDistribution() =>
            new(_delegates.Distribution(Left.GetProbabilityDistribution(), Right.GetProbabilityDistribution()));

        protected override IAssertion CreateAssertionWrapper() =>
            new AsAssertionWrapper(this);

        private LogicalProbabilityDistribution GetLogicalProbabilityDistribution() =>
            _delegates.AssertionDistribution(Left.GetProbabilityDistribution(), Right.GetProbabilityDistribution());

        private class AsAssertionWrapper : OperationAsAssertion
        {
            public AsAssertionWrapper(DefaultBinaryOperation operation) : base(operation) { }
            
            protected override LogicalProbabilityDistribution CreateProbabilityDistribution() =>
                ((DefaultBinaryOperation) _operation).GetLogicalProbabilityDistribution();
        }
    }
}
