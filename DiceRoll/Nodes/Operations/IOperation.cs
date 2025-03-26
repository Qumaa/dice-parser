namespace DiceRoll.Nodes
{
    public interface IOperation : IRollable, IDistributable<LogicalProbabilityDistribution, Logical> { }
}
