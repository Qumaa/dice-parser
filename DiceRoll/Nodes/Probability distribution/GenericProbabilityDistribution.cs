using System.Collections.Generic;

namespace DiceRoll.Nodes
{
    /// <summary>
    /// A general-purpose implementation of <see cref="ProbabilityDistribution{T}">ProbabilityDistribution</see>
    /// that guarantees the <see cref="Probability"/> field on its values. 
    /// </summary>
    /// <typeparam name="T"><inheritdoc cref="ProbabilityOf{T}"/></typeparam>
    public class GenericProbabilityDistribution<T> : ProbabilityDistribution<ProbabilityOf<T>>
    {
        /// <param name="probabilities">Sequence of <see cref="ProbabilityOf{T}"/> of type <typeparamref name="T"/>.</param>
        public GenericProbabilityDistribution(IEnumerable<ProbabilityOf<T>> probabilities) : base(probabilities) { }
    }
}
