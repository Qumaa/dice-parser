namespace DiceRoll.Nodes
{
    public sealed class DefaultOperation : Operation
    {
        private readonly OperationDelegates _delegates;

        public DefaultOperation(IAnalyzable left, IAnalyzable right, OperationType operationType) : base(left, right)
        {
            _delegates = DefaultOperationDelegates.Get(operationType);
        }

        public override LogicalProbabilityDistribution GetProbabilityDistribution() =>
            new(_delegates.Probability.Invoke(_left.GetProbabilityDistribution(), _right.GetProbabilityDistribution()));

        public override Binary Evaluate() =>
            _delegates.Evaluation(_left.Evaluate(), _right.Evaluate());

        public override OperationVerboseEvaluation EvaluateVerbose()
        {
            Outcome leftOutcome = _left.Evaluate();
            Outcome rightOutcome = _right.Evaluate();
            Binary operationOutcome = _delegates.Evaluation(leftOutcome, rightOutcome);
            
            return new OperationVerboseEvaluation(this, operationOutcome, _left, leftOutcome, _right, rightOutcome);
        }
    }
}
