using System;
using System.Collections.Generic;
using DiceRoll.Exceptions;

namespace DiceRoll.Expressions
{
    public sealed class Composite : IAnalyzable
    {
        private readonly IAnalyzable _composed;
        
        public Composite(IEnumerable<IAnalyzable> sequence, Composer composer)
        {
            EmptySequenceException.ThrowIfNullOrEmpty(sequence);
            ArgumentNullException.ThrowIfNull(composer);
            
            _composed = composer.Compose(sequence);
        }

        public Outcome Evaluate() =>
            _composed.Evaluate();

        public RollProbabilityDistribution GetProbabilityDistribution() =>
            _composed.GetProbabilityDistribution();
    }
}
