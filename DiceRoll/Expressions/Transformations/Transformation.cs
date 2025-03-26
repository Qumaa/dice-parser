namespace DiceRoll.Expressions
{
    public abstract class Transformation : IExpression<RollProbabilityDistribution>
    {
        protected readonly RollProbabilityDistribution _source;
        
        protected Transformation(RollProbabilityDistribution source)
        {
            _source = source;
        }

        public abstract RollProbabilityDistribution Evaluate();
    }
}
