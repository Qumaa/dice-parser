using System.Collections.Generic;

namespace DiceRoll
{
    public class GenericProbabilityDistribution<T> : ProbabilityDistribution<ProbabilityOf<T>>
    {
        /// <inheritdoc cref="ProbabilityDistribution{T}(IEnumerable{T})"/>
        public GenericProbabilityDistribution(IEnumerable<ProbabilityOf<T>> probabilities) : base(probabilities) { }
    }
}
