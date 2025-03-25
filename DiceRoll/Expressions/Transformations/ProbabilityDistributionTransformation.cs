namespace DiceRoll.Expressions
{
    public abstract class ProbabilityDistributionTransformation : IExpression<RollProbabilityDistribution>
    {
        protected readonly RollProbabilityDistribution _source;
        
        protected ProbabilityDistributionTransformation(RollProbabilityDistribution source)
        {
            _source = source;
        }

        public abstract RollProbabilityDistribution Evaluate();
    }
}
