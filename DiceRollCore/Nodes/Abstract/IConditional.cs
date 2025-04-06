namespace DiceRoll
{
    public interface IConditional : INode<Optional<Outcome>>, IDistributable<OptionalRollProbabilityDistribution, OptionalRoll> { }
}
