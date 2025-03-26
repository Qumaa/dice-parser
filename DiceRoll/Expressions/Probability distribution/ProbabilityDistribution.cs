using System.Collections;
using System.Collections.Generic;
using DiceRoll.Exceptions;

namespace DiceRoll.Expressions
{
    public abstract class ProbabilityDistribution<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _probabilities;
        
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
