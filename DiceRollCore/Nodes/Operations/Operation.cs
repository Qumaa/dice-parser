namespace DiceRoll
{
    public abstract class Operation : IOperation
    {
        public abstract Binary Evaluate();
        public abstract LogicalProbabilityDistribution GetProbabilityDistribution();
        public abstract OptionalRollProbabilityDistribution GetOptionalRollsProbabilityDistribution();

        public void Visit(INodeVisitor visitor) =>
            visitor.ForOperation(this);
    }
}
