namespace DiceRoll.Expressions
{
    public interface IAnalyzable : IRollable, IDistributable<RollProbabilityDistribution, Roll> { }
}
