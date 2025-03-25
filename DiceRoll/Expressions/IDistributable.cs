namespace DiceRoll.Expressions
{
    public interface IDistributable<T, TType> where T : ProbabilityDistribution<TType>
    {
        T GetProbabilityDistribution();
    }
}
