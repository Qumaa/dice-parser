namespace DiceRoll
{
    [BaseType("boolean")]
    public interface IAssertion : INode<Binary>, IDistributable<LogicalProbabilityDistribution, Logical>
    {
        Probability True { get; }
    }
}
