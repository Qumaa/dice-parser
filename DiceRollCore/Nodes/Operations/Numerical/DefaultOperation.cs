namespace DiceRoll
{
    public sealed class DefaultOperation : Operation
    {
        private readonly OperationDelegates _delegates;

        public DefaultOperation(INumeric left, INumeric right, OperationType operationType) : base(left, right)
        {
            _delegates = DefaultOperationDelegates.Get(operationType);
        }

        public override Optional<Outcome> Evaluate() =>
            _delegates.Evaluation(_left.Evaluate(), _right.Evaluate());

        public override OptionalRollProbabilityDistribution GetProbabilityDistribution() =>
            new(_delegates.Distribution(_left.GetProbabilityDistribution(), _right.GetProbabilityDistribution()));

        public override IAssertion AsAssertion() =>
            new AsAssertionWrapper(this);

        private LogicalProbabilityDistribution GetLogicalProbabilityDistribution() =>
            _delegates.AssertionEvaluation(_left.GetProbabilityDistribution(), _right.GetProbabilityDistribution());

        private class AsAssertionWrapper : Assertion
        {
            private readonly DefaultOperation _operation;
            
            public AsAssertionWrapper(DefaultOperation operation) 
            {
                _operation = operation;
            }
            
            public override Binary Evaluate() =>
                _operation.Evaluate().AsBinary();

            protected override LogicalProbabilityDistribution CreateProbabilityDistribution() =>
                _operation.GetLogicalProbabilityDistribution();
        }
    }
}
