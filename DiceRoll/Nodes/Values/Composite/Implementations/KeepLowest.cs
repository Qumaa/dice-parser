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
            IteratePairs(source, (left, right) => new Lowest(left, right));

        private sealed class Lowest : PairComposition
        {
            public Lowest(IAnalyzable left, IAnalyzable right) : base(left, right) { }
            
            public override Outcome Evaluate() =>
                Outcome.Min(_left.Evaluate(), _right.Evaluate());

            public override RollProbabilityDistribution GetProbabilityDistribution() =>
                Node.Transformation.SelectLowest(_left, _right).Evaluate();
        }
    }
}
