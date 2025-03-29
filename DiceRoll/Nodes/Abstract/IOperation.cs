namespace DiceRoll.Nodes
{
    public interface IOperation : INode<Binary>, IDistributable<LogicalProbabilityDistribution, Logical>
    {
        IAnalyzable GetNumeric();
    }
}
