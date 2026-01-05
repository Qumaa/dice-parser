namespace DiceRoll
{
    [BaseType("numerical")]
    public interface INumeric : INode<Outcome>, IDistributable<RollProbabilityDistribution, Roll> { }
}
