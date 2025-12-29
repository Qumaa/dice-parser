namespace DiceRoll
{
    public sealed class DefaultBinaryAssertion : BinaryAssertion
    {
        public readonly BinaryAssertionType AssertionType;
        private readonly BinaryOperationDelegates _delegates;

        public DefaultBinaryAssertion(IAssertion left, IAssertion right, BinaryAssertionType assertionType) :
            base(left, right)
        {
            _delegates = DefaultAssertionDelegates.Get(assertionType);
            AssertionType = assertionType;
        }

        public override void NextEvaluation()
        {
            Binary left = Left.Evaluate();
            Binary right = Right.Evaluate();
            
            CacheEvaluation(_delegates.Evaluation.Invoke(left, right));
        }
        
        protected override LogicalProbabilityDistribution CreateProbabilityDistribution() =>
            new(_delegates.Probability.Invoke(Left.GetProbabilityDistribution(), Right.GetProbabilityDistribution()));
    }
}
