namespace DiceRoll.Expressions
{
    public interface IAnalyzable : IRollable
    {
        ProbabilityDistribution GetProbabilityDistribution();
    }
}
