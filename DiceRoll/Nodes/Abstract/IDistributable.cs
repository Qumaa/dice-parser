namespace DiceRoll.Nodes
{
    public interface IDistributable<T, TType> where T : ProbabilityDistribution<TType>
    {
        T GetProbabilityDistribution();
    }
}
