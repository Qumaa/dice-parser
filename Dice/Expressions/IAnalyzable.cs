namespace Dice.Expressions
{
    public interface IAnalyzable : IRollable
    {
        ProbabilityDistribution GetProbabilityData();
    }
}
