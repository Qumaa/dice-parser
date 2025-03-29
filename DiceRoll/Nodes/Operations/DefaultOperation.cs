using System;

namespace DiceRoll.Nodes
{
    public sealed class DefaultOperation : IOperation
    {
        private readonly IAnalyzable _left;
        private readonly IAnalyzable _right;
        private readonly OperationDelegates _delegates;

        public DefaultOperation(IAnalyzable left, IAnalyzable right, OperationType operationType)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);
            
            _left = left;
            _right = right;
            _delegates = DefaultOperationDelegates.Get(operationType);
        }

        public LogicalProbabilityDistribution GetProbabilityDistribution() =>
            new(_delegates.Probability.Invoke(_left.GetProbabilityDistribution(), _right.GetProbabilityDistribution()));

        public Binary Evaluate() =>
            _delegates.Evaluation(_left.Evaluate(), _right.Evaluate());

        public IAnalyzable GetNumeric() =>
            _left;
    }
}
