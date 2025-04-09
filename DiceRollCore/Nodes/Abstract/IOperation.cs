namespace DiceRoll
{
    public interface IOperation : INode<Optional<Outcome>>,
        IDistributable<OptionalRollProbabilityDistribution, OptionalRoll>
    {
        IAssertion AsAssertion();
    }
}
