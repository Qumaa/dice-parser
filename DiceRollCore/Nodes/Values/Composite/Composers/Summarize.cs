namespace DiceRoll
{
    /// <summary>
    /// Composes the sequence of <see cref="INumeric">numerical nodes</see>,
    /// using the <see cref="Combination"/> node returned by
    /// <see cref="Node.Operator.Add(RollProbabilityDistribution, RollProbabilityDistribution)">Add</see>.
    /// </summary>
    public sealed class Summarize : Composer
    {
        protected override INumeric Compose(INumeric[] source) =>
            IteratePairs(source, static (left, right) => Node.Operator.Add(left, right));
    }
}
