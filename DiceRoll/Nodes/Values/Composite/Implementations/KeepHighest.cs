namespace DiceRoll.Nodes
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
            IteratePairs(source, (left, right) => new Highest(left, right));

        private sealed class Highest : PairComposition
        {
            public Highest(IAnalyzable left, IAnalyzable right) : base(left, right) { }
            
            public override Outcome Evaluate() =>
                Outcome.Max(_left.Evaluate(), _right.Evaluate());

            public override RollProbabilityDistribution GetProbabilityDistribution() =>
                Node.Transformation.SelectHighest(_left, _right).Evaluate();
        }
    }
}
