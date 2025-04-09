using System;

namespace DiceRoll
{
    /// <summary>
    /// Base class that allows to wrap an arbitrary <see cref="INumeric">numeric node</see> to transform its
    /// <see cref="Outcome"/> and <see cref="RollProbabilityDistribution">probability distribution</see>
    /// of the results.
    /// </summary>
    public abstract class Transformation : Numeric
    {
        /// <summary>
        /// The main <see cref="INumeric">numeric node</see>.
        /// </summary>
        protected readonly INumeric _source;
        
        /// <param name="source">The main <see cref="INumeric">numeric node</see>.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="source"/> is null.</exception>
        protected Transformation(INumeric source)
        {
            ArgumentNullException.ThrowIfNull(source);
            
            _source = source;
        }

        public abstract override Outcome Evaluate();

        protected abstract override RollProbabilityDistribution CreateProbabilityDistribution();
    }
}
