using System;

namespace DiceRoll.Nodes
{
    /// <summary>
    /// Base class that allows to wrap an arbitrary <see cref="IAnalyzable">numeric node</see> to transform its
    /// <see cref="Outcome"/> and <see cref="RollProbabilityDistribution">probability distribution</see>
    /// of the results.
    /// </summary>
    public abstract class Transformation : IAnalyzable
    {
        /// <summary>
        /// The main <see cref="IAnalyzable">numeric node</see>.
        /// </summary>
        protected readonly IAnalyzable _source;
        
        /// <param name="source">The main <see cref="IAnalyzable">numeric node</see>.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="source"/> is null.</exception>
        protected Transformation(IAnalyzable source)
        {
            ArgumentNullException.ThrowIfNull(source);
            
            _source = source;
        }

        public abstract Outcome Evaluate();
        
        public abstract RollProbabilityDistribution GetProbabilityDistribution();
    }
}
