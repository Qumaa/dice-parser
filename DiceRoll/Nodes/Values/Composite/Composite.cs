using System;
using System.Collections.Generic;
using DiceRoll.Exceptions;

namespace DiceRoll.Nodes
{
    /// <summary>
    /// A <see cref="IAnalyzable">numerical node</see> that combines an arbitrary sequence of
    /// other numerical nodes, using a <see cref="Composer"/> implementation instance.
    /// </summary>
    public sealed class Composite : IAnalyzable
    {
        private readonly IAnalyzable _composite;
        
        /// <param name="sequence">Nodes to be combined.</param>
        /// <param name="composer">A <see cref="Composer"/> instance that determines how the <paramref name="sequence"/>
        /// is combined.</param>
        /// <exception cref="ArgumentNullException">When either <paramref name="sequence"/> or
        /// <paramref name="composer"/> are null.</exception>
        /// <exception cref="EmptySequenceException">When <paramref name="sequence"/> is empty.</exception>
        public Composite(IEnumerable<IAnalyzable> sequence, Composer composer)
        {
            EmptySequenceException.ThrowIfNullOrEmpty(sequence);
            ArgumentNullException.ThrowIfNull(composer);
            
            _composite = composer.Compose(sequence);
        }

        public Outcome Evaluate() =>
            _composite.Evaluate();

        public RollProbabilityDistribution GetProbabilityDistribution() =>
            _composite.GetProbabilityDistribution();
    }
}
