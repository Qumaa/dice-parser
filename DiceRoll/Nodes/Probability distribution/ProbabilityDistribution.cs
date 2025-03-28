using System;
using System.Collections;
using System.Collections.Generic;
using DiceRoll.Exceptions;

namespace DiceRoll.Nodes
{
    /// <summary>
    /// Base class that represents a range of values along with their probabilities.
    /// </summary>
    /// <typeparam name="T">Type of contained values. Usually contains a field of type <see cref="Probability"/> (not enforced).</typeparam>
    public abstract class ProbabilityDistribution<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _probabilities;
        
        /// <param name="probabilities">Sequence of <typeparamref name="T"/>.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="probabilities"/> is null.</exception>
        /// <exception cref="EmptySequenceException">When <paramref name="probabilities"/> is empty.</exception>
        protected ProbabilityDistribution(IEnumerable<T> probabilities) 
        {
            EmptySequenceException.ThrowIfNullOrEmpty(probabilities);
            
            _probabilities = probabilities;
        }

        public IEnumerator<T> GetEnumerator() =>
            _probabilities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}
