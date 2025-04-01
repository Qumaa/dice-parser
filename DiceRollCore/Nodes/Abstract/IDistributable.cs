namespace DiceRoll.Nodes
{
    /// <summary>
    /// Base interface for any range of arbitrary values to analyze or transform their probabilities.
    /// </summary>
    /// <typeparam name="T">Any implementation of <see cref="ProbabilityDistribution{T}"/>.</typeparam>
    /// <typeparam name="TType">Type of values that <typeparamref name="T"/> holds.</typeparam>
    public interface IDistributable<out T, TType> where T : ProbabilityDistribution<TType>
    {
        /// <summary>
        /// Creates a <see cref="ProbabilityDistribution{T}">ProbabilityDistribution</see> instance of type
        /// <typeparamref name="T"/>.
        /// </summary>
        /// <returns>A <typeparamref name="T"/> instance.</returns>
        T GetProbabilityDistribution();
    }
}
