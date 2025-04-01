using System;

namespace DiceRoll.Nodes
{
    /// <summary>
    /// Base class that allows to wrap two arbitrary <see cref="IAnalyzable">numeric nodes</see> to perform binary
    /// operations on their <see cref="Outcome"/> and provide a
    /// <see cref="RollProbabilityDistribution">probability distribution</see> of true and false of said operation.
    /// </summary>
    public abstract class NumericOperation : IOperation
    {
        protected readonly IAnalyzable _left;
        protected readonly IAnalyzable _right;

        protected NumericOperation(IAnalyzable left, IAnalyzable right)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);
            
            _left = left;
            _right = right;
        }

        public abstract Binary Evaluate();

        public abstract LogicalProbabilityDistribution GetProbabilityDistribution();
    }
}
