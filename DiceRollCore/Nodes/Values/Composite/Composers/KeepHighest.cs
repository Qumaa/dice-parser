namespace DiceRoll
{
    /// <summary>
    /// Composes the sequence of <see cref="INumeric">numerical nodes</see>,
    /// using the <see cref="Selection"/> node returned by
    /// <see cref="Node.Operator.SelectHighest(RollProbabilityDistribution, RollProbabilityDistribution)">
    /// SelectHighest</see>.
    /// </summary>
    public sealed class KeepHighest : Composer
    {
        protected override INumeric Compose(INumeric[] source) =>
            IteratePairs(source, Node.Operator.SelectHighest);
    }
}
