using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll
{
    /// <summary>
    /// A <see cref="INumeric">numerical node</see> that combines an arbitrary sequence of
    /// other numerical nodes, using a <see cref="Composer"/> implementation instance.
    /// </summary>
    public sealed class Composite : NumericNode
    {
        private readonly INumeric _composite;
        
        /// <param name="sequence">Nodes to be combined.</param>
        /// <param name="composer">A <see cref="Composer"/> instance that determines how the <paramref name="sequence"/>
        /// is combined.</param>
        /// <exception cref="ArgumentNullException">When either <paramref name="sequence"/> or
        /// <paramref name="composer"/> are null.</exception>
        /// <exception cref="EmptyEnumerableException">When <paramref name="sequence"/> is empty.</exception>
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

        public override RollProbabilityDistribution GetProbabilityDistribution() =>
            _composite.GetProbabilityDistribution();
    }
}
