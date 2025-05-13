namespace DiceRoll
{
    public sealed class DefaultBinaryAssertion : BinaryAssertion
    {
        private readonly BinaryOperationDelegates _delegates;

        public DefaultBinaryAssertion(IAssertion left, IAssertion right, BinaryAssertionType assertionType) :
            base(left, right)
        {
            _delegates = DefaultAssertionDelegates.Get(assertionType);
        }
        
        public override void Next()
        {
            Binary left = _left.Evaluate();
            Binary right = _right.Evaluate();
            
            CacheEvaluation(_delegates.Evaluation.Invoke(left, right));
        }

        protected override LogicalProbabilityDistribution CreateProbabilityDistribution() =>
            new(_delegates.Probability.Invoke(_left.GetProbabilityDistribution(), _right.GetProbabilityDistribution()));
    }
}
