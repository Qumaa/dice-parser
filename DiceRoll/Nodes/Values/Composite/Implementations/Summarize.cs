namespace DiceRoll.Nodes
{
    /// <summary>
    /// Composes the sequence of <see cref="IAnalyzable">numerical nodes</see>,
    /// using the <see cref="Combination"/> node returned by
    /// <see cref="Node.Transformation.Add(RollProbabilityDistribution, RollProbabilityDistribution)">Add</see>.
    /// </summary>
    public sealed class Summarize : Composer
    {
        protected override IAnalyzable Compose(IAnalyzable[] source) =>
            IteratePairs(source, (left, right) => new Sum(left, right));

        private sealed class Sum : PairComposition
        {
            public Sum(IAnalyzable left, IAnalyzable right) : base(left, right) { }

            public override Outcome Evaluate() =>
                _left.Evaluate() + _right.Evaluate();

            public override RollProbabilityDistribution GetProbabilityDistribution() =>
                Node.Transformation.Add(_left, _right).Evaluate();
        }
    }
}
