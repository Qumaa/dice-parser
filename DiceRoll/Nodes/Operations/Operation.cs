using System;

namespace DiceRoll.Nodes
{
    public sealed class Operation : IOperation
    {
        private readonly IAnalyzable _left;
        private readonly IAnalyzable _right;
        private readonly OperationDelegate _operationDelegate;

        public Operation(IAnalyzable left, IAnalyzable right, OperationDelegate operationDelegate)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);
            ArgumentNullException.ThrowIfNull(operationDelegate);
            
            _left = left;
            _right = right;
            _operationDelegate = operationDelegate;
        }

        public LogicalProbabilityDistribution GetProbabilityDistribution() =>
            new(_operationDelegate.Invoke(_left.GetProbabilityDistribution(), _right.GetProbabilityDistribution()));

        public Outcome Evaluate() =>
            _left.Evaluate();
    }
}
