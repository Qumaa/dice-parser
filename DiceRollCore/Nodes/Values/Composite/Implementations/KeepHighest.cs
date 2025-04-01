namespace DiceRoll
{
    /// <summary>
    /// Composes the sequence of <see cref="IAnalyzable">numerical nodes</see>,
    /// using the <see cref="Selection"/> node returned by
    /// <see cref="Node.Transformation.SelectHighest(RollProbabilityDistribution, RollProbabilityDistribution)">
    /// SelectHighest</see>.
    /// </summary>
    public sealed class KeepHighest : Composer
    {
        protected override IAnalyzable Compose(IAnalyzable[] source) =>
            IteratePairs(source, Node.Transformation.SelectHighest);
    }
}
