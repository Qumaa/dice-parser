using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll
{
    public sealed class Composite : Numeric
    {
        private readonly INumeric _composite;
        
        public Composite(IEnumerable<INumeric> sequence, Composer composer)
        {
            EmptyEnumerableException.ThrowIfNullOrEmpty(sequence);
            ArgumentNullException.ThrowIfNull(composer);
            
            _composite = composer.Compose(sequence);
        }

        public Composite(INumeric node, int repetitionCount, Composer composer) :
            this(Enumerable.Repeat(node, CompositeRepetitionException.ThrowIfBelowTwo(repetitionCount)), composer) { }

        public override Outcome Evaluate() =>
            _composite.Evaluate();

        protected override RollProbabilityDistribution CreateProbabilityDistribution() =>
            _composite.GetProbabilityDistribution();
    }
}
