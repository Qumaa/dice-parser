using System;
using System.Collections;
using System.Collections.Generic;

namespace DiceRoll
{
    public abstract class ProbabilityDistribution<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _probabilities;
        
        protected ProbabilityDistribution(IEnumerable<T> probabilities) 
        {
            EmptyEnumerableException.ThrowIfNullOrEmpty(probabilities);
            
            _probabilities = probabilities;
        }

        public IEnumerator<T> GetEnumerator() =>
            _probabilities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}
