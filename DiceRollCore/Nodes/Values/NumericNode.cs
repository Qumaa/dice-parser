namespace DiceRoll
{
    public abstract class NumericNode : INumeric
    {
        public abstract Outcome Evaluate();

        public abstract RollProbabilityDistribution GetProbabilityDistribution();

        public void Visit(INodeVisitor visitor) =>
            visitor.ForNumeric(this);
    }
}
