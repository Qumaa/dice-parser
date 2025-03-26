namespace DiceRoll.Expressions
{
    public interface IOperation : IRollable, IDistributable<LogicalProbabilityDistribution, Logical> { }
}
