namespace DiceRoll
{
    /// <summary>
    /// Composes the sequence of <see cref="INumeric">numerical nodes</see>,
    /// using the <see cref="Combination"/> node returned by
    /// <see cref="Node.Transformation.Add(RollProbabilityDistribution, RollProbabilityDistribution)">Add</see>.
    /// </summary>
    public sealed class Summarize : Composer
    {
        protected override INumeric Compose(INumeric[] source) =>
            IteratePairs(source, Node.Transformation.Add);
    }
}
