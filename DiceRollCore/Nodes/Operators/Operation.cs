using System;

namespace DiceRoll
{
    public abstract class Operation : IOperation
    {
        private OptionalRollProbabilityDistribution _cachedDistribution;

        public Optional<Outcome> Evaluation { get; private set; }

        public IAssertion AsAssertion { get; }

        protected Operation()
        {
            AsAssertion = CreateAssertionWrapperSafe();
        }

        public abstract void Next();

        public OptionalRollProbabilityDistribution GetProbabilityDistribution() =>
            _cachedDistribution ??= CreateProbabilityDistribution();

        public void Visit<T>(T visitor) where T : INodeVisitor =>
            visitor.ForOperation(this);

        protected void CacheEvaluation(in Optional<Outcome> evaluation) =>
            Evaluation = evaluation;
        
        protected virtual IAssertion CreateAssertionWrapper() =>
            DefaultAssertionFactory();

        protected abstract OptionalRollProbabilityDistribution CreateProbabilityDistribution();

        private IAssertion CreateAssertionWrapperSafe()
        {
            try
            {
                return CreateAssertionWrapper() ?? DefaultAssertionFactory();
            }
            catch (Exception)
            {
                return DefaultAssertionFactory();
            }
        }

        private OperationAsAssertion DefaultAssertionFactory() =>
            new(this);
    }
}
