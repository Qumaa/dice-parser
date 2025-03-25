namespace DiceRoll.Expressions
{
    public interface IOperation : IRollable, IDistributable<BinaryProbabilityDistribution, Binary> { }
}
