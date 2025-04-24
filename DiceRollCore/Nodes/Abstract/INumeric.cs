namespace DiceRoll
{
    public interface INumeric : INode<Outcome>, IDistributable<RollProbabilityDistribution, Roll> { }
}
