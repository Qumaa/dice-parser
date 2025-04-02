namespace DiceRoll
{
    public abstract class Operation : IOperation
    {
        public abstract Binary Evaluate();
        public abstract LogicalProbabilityDistribution GetProbabilityDistribution();
        public void Visit(INodeVisitor visitor) =>
            visitor.ForOperation(this);
    }
}
