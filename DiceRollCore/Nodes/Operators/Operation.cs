using System;

namespace DiceRoll
{
    public abstract class Operation : IOperation, IAssertion
    {
        private readonly IAssertion _asAssertion;

        protected Operation()
        {
            _asAssertion = AsAssertionSafe();
        }

        public abstract Optional<Outcome> Evaluate();
        public abstract OptionalRollProbabilityDistribution GetProbabilityDistribution();

        public virtual IAssertion AsAssertion() =>
            DefaultAsAssertion();

        public void Visit(INodeVisitor visitor) =>
            visitor.ForOperation(this);

        private IAssertion AsAssertionSafe()
        {
            try
            {
                return AsAssertion();
            }
            catch (Exception)
            {
                return DefaultAsAssertion();
            }
        }

        private OperationAsAssertion DefaultAsAssertion() =>
            new(this);

        Probability IAssertion.True => _asAssertion.True;

        Binary INode<Binary>.Evaluate() =>
            _asAssertion.Evaluate();

        LogicalProbabilityDistribution IDistributable<LogicalProbabilityDistribution, Logical>.GetProbabilityDistribution() =>
            _asAssertion.GetProbabilityDistribution();
    }
}
