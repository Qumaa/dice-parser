namespace DiceRoll
{
    public sealed class DefaultBinaryOperation : BinaryOperation
    {
        private readonly OperationDelegates _delegates;

        public DefaultBinaryOperation(INumeric left, INumeric right, OperationType operationType) : base(left, right)
        {
            _delegates = DefaultOperationDelegates.Get(operationType);
        }

        public override void NextEvaluation()
        {
            Outcome left = _left.Evaluate();
            Outcome right = _right.Evaluate();
            
            CacheEvaluation(_delegates.Evaluation(left, right));
        }

        protected override OptionalRollProbabilityDistribution CreateProbabilityDistribution() =>
            new(_delegates.Distribution(_left.GetProbabilityDistribution(), _right.GetProbabilityDistribution()));

        protected override IAssertion CreateAssertionWrapper() =>
            new AsAssertionWrapper(this);

        private LogicalProbabilityDistribution GetLogicalProbabilityDistribution() =>
            _delegates.AssertionDistribution(_left.GetProbabilityDistribution(), _right.GetProbabilityDistribution());

        private class AsAssertionWrapper : OperationAsAssertion
        {
            public AsAssertionWrapper(DefaultBinaryOperation operation) : base(operation) { }
            
            protected override LogicalProbabilityDistribution CreateProbabilityDistribution() =>
                ((DefaultBinaryOperation) _operation).GetLogicalProbabilityDistribution();
        }
    }
}
