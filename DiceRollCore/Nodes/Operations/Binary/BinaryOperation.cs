using System;

namespace DiceRoll
{
    public abstract class BinaryOperation : Operation
    {
        protected readonly IOperation _left;
        protected readonly IOperation _right;

        protected BinaryOperation(IOperation left, IOperation right)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);
            
            _left = left;
            _right = right;
        }

        public abstract override Binary Evaluate();
        
        public abstract override LogicalProbabilityDistribution GetProbabilityDistribution();
    }
}
