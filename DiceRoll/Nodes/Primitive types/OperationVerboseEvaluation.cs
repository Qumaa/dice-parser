namespace DiceRoll.Nodes
{
    public sealed class OperationVerboseEvaluation
    {
        public readonly Binary OperationOutcome;
        public readonly Outcome LeftOutcome;
        public readonly Outcome RightOutcome;
        
        private readonly IOperation _operation;
        private readonly IAnalyzable _left;
        private readonly IAnalyzable _right;
        
        public OperationVerboseEvaluation(IOperation operation, Binary operationOutcome, IAnalyzable left,
            Outcome leftOutcome, IAnalyzable right, Outcome rightOutcome)
        {
            _operation = operation;
            OperationOutcome = operationOutcome;
            
            _left = left;
            LeftOutcome = leftOutcome;
            
            _right = right;
            RightOutcome = rightOutcome;
        }

        public LogicalProbabilityDistribution GetLogicalProbabilityDistribution() =>
            _operation.GetProbabilityDistribution();
        
        public RollProbabilityDistribution GetLeftProbabilityDistribution() =>
            _left.GetProbabilityDistribution();
        
        public RollProbabilityDistribution GetRightProbabilityDistribution() =>
            _right.GetProbabilityDistribution();
    }
}
