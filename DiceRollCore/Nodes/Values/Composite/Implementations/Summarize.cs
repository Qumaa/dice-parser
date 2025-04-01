namespace DiceRoll
{
    /// <summary>
    /// Composes the sequence of <see cref="IAnalyzable">numerical nodes</see>,
    /// using the <see cref="Combination"/> node returned by
    /// <see cref="Node.Transformation.Add(RollProbabilityDistribution, RollProbabilityDistribution)">Add</see>.
    /// </summary>
    public sealed class Summarize : Composer
    {
        protected override IAnalyzable Compose(IAnalyzable[] source) =>
            IteratePairs(source, Node.Transformation.Add);
    }
}
