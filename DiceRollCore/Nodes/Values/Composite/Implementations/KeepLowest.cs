namespace DiceRoll.Nodes
{
    /// <summary>
    /// Composes the sequence of <see cref="IAnalyzable">numerical nodes</see>,
    /// using the <see cref="Selection"/> node returned by
    /// <see cref="Node.Transformation.SelectLowest(RollProbabilityDistribution, RollProbabilityDistribution)">
    /// SelectLowest</see>.
    /// </summary>
    public sealed class KeepLowest : Composer
    {
        protected override IAnalyzable Compose(IAnalyzable[] source) =>
            IteratePairs(source, Node.Transformation.SelectLowest);
    }
}
