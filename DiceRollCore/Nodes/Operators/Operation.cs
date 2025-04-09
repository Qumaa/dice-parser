using System;

namespace DiceRoll
{
    public abstract class Operation : IOperation, IAssertion
    {
        private readonly IAssertion _asAssertion;
        private OptionalRollProbabilityDistribution _cachedDistribution;

        protected Operation()
        {
            _asAssertion = CreateAssertionWrapperSafe();
        }

        public abstract Optional<Outcome> Evaluate();

        public OptionalRollProbabilityDistribution GetProbabilityDistribution() =>
            _cachedDistribution ??= CreateProbabilityDistribution();

        public virtual IAssertion AsAssertion() =>
            _asAssertion;

        public void Visit(INodeVisitor visitor) =>
            visitor.ForOperation(this);

        protected virtual Assertion CreateAssertionWrapper() =>
            DefaultAssertionFactory();

        protected abstract OptionalRollProbabilityDistribution CreateProbabilityDistribution();

        private IAssertion CreateAssertionWrapperSafe()
        {
            try
            {
                return CreateAssertionWrapper();
            }
            catch (Exception)
            {
                return DefaultAssertionFactory();
            }
        }

        private OperationAsAssertion DefaultAssertionFactory() =>
            new(this);

        Probability IAssertion.True => _asAssertion.True;

        Binary INode<Binary>.Evaluate() =>
            _asAssertion.Evaluate();

        LogicalProbabilityDistribution IDistributable<LogicalProbabilityDistribution, Logical>.
            GetProbabilityDistribution() =>
            _asAssertion.GetProbabilityDistribution();
    }
}
