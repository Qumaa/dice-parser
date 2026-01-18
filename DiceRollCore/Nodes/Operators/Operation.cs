using System;

namespace DiceRoll
{
    public abstract class Operation : IOperation
    {
        private OptionalRollProbabilityDistribution _cachedDistribution;

        public Optional<Outcome> CachedEvaluation { get; private set; }

        public IAssertion AsAssertion { get; }

        protected Operation()
        {
            AsAssertion = CreateAssertionWrapperSafe();
        }

        public abstract void NextEvaluation();

        public OptionalRollProbabilityDistribution GetProbabilityDistribution() =>
            _cachedDistribution ??= CreateProbabilityDistribution();

        public virtual void Visit<T>(T visitor) where T : INodeVisitor =>
            visitor.ForOperation(this);

        public virtual object Clone() =>
            MemberwiseClone();

        protected void CacheEvaluation(in Optional<Outcome> evaluation) =>
            CachedEvaluation = evaluation;

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
