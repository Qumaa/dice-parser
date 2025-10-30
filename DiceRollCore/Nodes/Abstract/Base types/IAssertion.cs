namespace DiceRoll
{
    public interface IAssertion : INode<Binary>, IDistributable<LogicalProbabilityDistribution, Logical>
    {
        Probability True { get; }
    }
}
