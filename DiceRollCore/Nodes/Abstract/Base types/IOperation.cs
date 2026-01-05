namespace DiceRoll
{
    [BaseType("numerical comparison")]
    public interface IOperation : INode<Optional<Outcome>>,
        IDistributable<OptionalRollProbabilityDistribution, OptionalRoll>
    {
        IAssertion AsAssertion { get; }
    }
}
