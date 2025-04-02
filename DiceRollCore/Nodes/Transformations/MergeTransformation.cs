using System;

namespace DiceRoll
{
    /// <summary>
    /// A base class for any <see cref="Transformation"/> that merges two arbitrary
    /// <see cref="INumeric">numeric nodes</see> instead of transforming a single node.
    /// </summary>
    public abstract class MergeTransformation : Transformation
    {
        /// <summary>
        /// The second <see cref="INumeric">numeric node</see> to be merged with.
        /// </summary>
        protected readonly INumeric _other;

        /// <param name="source">The main <see cref="INumeric">numeric node</see>.</param>
        /// <param name="other">
        /// The second <see cref="INumeric">numeric node</see> to be merged with <paramref name="source"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// When either <paramref name="source"/> or <paramref name="other"/> is null.
        /// </exception>
        protected MergeTransformation(INumeric source, INumeric other) : base(source)
        {
            ArgumentNullException.ThrowIfNull(other);
            
            _other = other;
        }
    }
}
