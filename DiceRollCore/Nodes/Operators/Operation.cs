using System;

namespace DiceRoll
{
    public abstract class Operation : IOperation, IAssertion
    {
        private readonly IAssertion _asAssertion;
        private OptionalRollProbabilityDistribution _cachedDistribution;

        public Optional<Outcome> Evaluation { get; private set; }

        protected Operation()
        {
            _asAssertion = CreateAssertionWrapperSafe();
        }

        public abstract void Next();

        public OptionalRollProbabilityDistribution GetProbabilityDistribution() =>
            _cachedDistribution ??= CreateProbabilityDistribution();

        public IAssertion AsAssertion() =>
            _asAssertion;

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

        Binary INode<Binary>.Evaluation => _asAssertion.Evaluation;

        LogicalProbabilityDistribution IDistributable<LogicalProbabilityDistribution, Logical>.
            GetProbabilityDistribution() =>
            _asAssertion.GetProbabilityDistribution();
    }
}
