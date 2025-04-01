namespace DiceRoll
{
    /// <summary>
    /// A base class for any <see cref="Transformation"/> that merges two arbitrary
    /// <see cref="IAnalyzable">numeric nodes</see> instead of transforming a single node.
    /// </summary>
    public abstract class MergeTransformation : Transformation
    {
        /// <summary>
        /// The second <see cref="IAnalyzable">numeric node</see> to be merged with.
        /// </summary>
        protected readonly IAnalyzable _other;

        /// <param name="source">The main <see cref="IAnalyzable">numeric node</see>.</param>
        /// <param name="other">
        /// The second <see cref="IAnalyzable">numeric node</see> to be merged with <paramref name="source"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// When either <paramref name="source"/> or <paramref name="other"/> is null.
        /// </exception>
        protected MergeTransformation(IAnalyzable source, IAnalyzable other) : base(source)
        {
            ArgumentNullException.ThrowIfNull(other);
            
            _other = other;
        }
    }
}
