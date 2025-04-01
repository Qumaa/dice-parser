using System.Collections.Generic;

namespace DiceRoll
{
    /// <summary>
    /// A general-purpose implementation of <see cref="ProbabilityDistribution{T}">ProbabilityDistribution</see>
    /// that guarantees the <see cref="Probability"/> field on its values. 
    /// </summary>
    /// <typeparam name="T"><inheritdoc cref="ProbabilityOf{T}"/></typeparam>
    public class GenericProbabilityDistribution<T> : ProbabilityDistribution<ProbabilityOf<T>>
    {
        /// <inheritdoc cref="ProbabilityDistribution{T}(IEnumerable{T})"/>
        public GenericProbabilityDistribution(IEnumerable<ProbabilityOf<T>> probabilities) : base(probabilities) { }
    }
}
