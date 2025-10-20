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
            _delegates.AssertionEvaluation(_left.GetProbabilityDistribution(), _right.GetProbabilityDistribution());

        private class AsAssertionWrapper : Assertion
        {
            private readonly DefaultBinaryOperation _operation;

            public AsAssertionWrapper(DefaultBinaryOperation operation)
            {
                _operation = operation;
            }
            
            public override void NextEvaluation()
            {
                _operation.NextEvaluation();
                CacheEvaluation(_operation.CachedEvaluation.AsBinary());
            }
            
            protected override LogicalProbabilityDistribution CreateProbabilityDistribution() =>
                _operation.GetLogicalProbabilityDistribution();
        }
    }
}
