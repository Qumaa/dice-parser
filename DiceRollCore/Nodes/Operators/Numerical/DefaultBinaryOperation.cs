namespace DiceRoll
{
    public sealed class DefaultBinaryOperation : BinaryOperation
    {
        private readonly OperationDelegates _delegates;

        public DefaultBinaryOperation(INumeric left, INumeric right, OperationType operationType) : base(left, right)
        {
            _delegates = DefaultOperationDelegates.Get(operationType);
        }

        protected override Optional<Outcome> GetNextEvaluation() =>
            _delegates.Evaluation(_left.Evaluation, _right.Evaluation);

        protected override OptionalRollProbabilityDistribution CreateProbabilityDistribution() =>
            new(_delegates.Distribution(_left.GetProbabilityDistribution(), _right.GetProbabilityDistribution()));

        protected override Assertion CreateAssertionWrapper() =>
            new AsAssertionWrapper(this);

        private LogicalProbabilityDistribution GetLogicalProbabilityDistribution() =>
            _delegates.AssertionEvaluation(_left.GetProbabilityDistribution(), _right.GetProbabilityDistribution());

        private class AsAssertionWrapper : Assertion
        {
            private readonly DefaultBinaryOperation _operation;

            public AsAssertionWrapper(DefaultBinaryOperation operation)
            {
                _operation = operation;
            }

            protected override Binary GetNextEvaluation() =>
                _operation.GetNextEvaluation().AsBinary();

            protected override LogicalProbabilityDistribution CreateProbabilityDistribution() =>
                _operation.GetLogicalProbabilityDistribution();
        }
    }
}
