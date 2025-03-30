using System;

namespace DiceRoll.Nodes
{
    public sealed class DefaultBinaryOperation : BinaryOperation
    {
        private readonly BinaryOperationDelegates _delegates;
        
        public DefaultBinaryOperation(IOperation left, IOperation right, BinaryOperationType operationType) : base(left, right)
        {
            _delegates = DefaultBinaryOperationDelegates.Get(operationType);
        }
        
        public override Binary Evaluate() =>
            _delegates.Evaluation.Invoke(_left.Evaluate(), _right.Evaluate());

        public override LogicalProbabilityDistribution GetProbabilityDistribution() =>
            new(_delegates.Probability.Invoke(_left.GetProbabilityDistribution(), _right.GetProbabilityDistribution()));
    }
}
