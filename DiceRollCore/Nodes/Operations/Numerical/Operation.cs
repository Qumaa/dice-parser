using System;

namespace DiceRoll
{
    /// <summary>
    /// Base class that allows to wrap two arbitrary <see cref="INumeric">numeric nodes</see> to perform binary
    /// operations on their <see cref="Outcome"/> and provide a
    /// <see cref="RollProbabilityDistribution">probability distribution</see> of true and false of said assertion.
    /// </summary>
    public abstract class Operation : IOperation
    {
        protected readonly INumeric _left;
        protected readonly INumeric _right;

        protected Operation(INumeric left, INumeric right)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);
            
            _left = left;
            _right = right;
        }

        public abstract Optional<Outcome> Evaluate();

        public abstract OptionalRollProbabilityDistribution GetProbabilityDistribution();
        
        public virtual IAssertion AsAssertion() =>
            new OperationAsAssertion(this);

        public void Visit(INodeVisitor visitor) =>
            visitor.ForOperation(this);
    }
}
