namespace DiceRoll.Expressions
{
    public abstract class ProbabilityDistributionTransformation : IExpression<ProbabilityDistribution>
    {
        protected readonly ProbabilityDistribution _source;
        
        protected ProbabilityDistributionTransformation(ProbabilityDistribution source)
        {
            _source = source;
        }

        public abstract ProbabilityDistribution Evaluate();
    }
}
