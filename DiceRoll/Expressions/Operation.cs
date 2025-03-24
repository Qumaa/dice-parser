namespace DiceRoll.Expressions
{
    public sealed class Operation : IExpression<Probability>
    {
        private readonly IAnalyzable _left;
        private readonly IAnalyzable _right;
        private readonly OperationDelegate _operationDelegate;

        public Operation(IAnalyzable left, OperationDelegate operationDelegate, IAnalyzable right)
        {
            _left = left;
            _right = right;
            _operationDelegate = operationDelegate;
        }

        public Probability Evaluate() =>
            _operationDelegate.Invoke(_left.GetProbabilityDistribution(), _right.GetProbabilityDistribution());

    }
}
