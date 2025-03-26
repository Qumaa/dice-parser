namespace DiceRoll.Nodes
{
    public interface IAnalyzable : IRollable, IDistributable<RollProbabilityDistribution, Roll> { }
}
