namespace DiceRoll
{
    /// <summary>
    /// Composes the sequence of <see cref="INumeric">numerical nodes</see>,
    /// using the <see cref="Selection"/> node returned by
    /// <see cref="Node.Operator.SelectLowest(RollProbabilityDistribution, RollProbabilityDistribution)">
    /// SelectLowest</see>.
    /// </summary>
    public sealed class KeepLowest : Composer
    {
        protected override INumeric Compose(INumeric[] source) =>
            IteratePairs(source, static (left, right) => Node.Operator.SelectLowest(left, right));
    }
}
