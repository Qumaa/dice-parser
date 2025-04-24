namespace DiceRoll
{
    public interface IDistributable<out T, TType> where T : ProbabilityDistribution<TType>
    {
        T GetProbabilityDistribution();
    }
}
