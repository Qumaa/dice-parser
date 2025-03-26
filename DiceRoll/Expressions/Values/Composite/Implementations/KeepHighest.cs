namespace DiceRoll.Expressions
{
    public sealed class KeepHighest : Composer
    {
        protected override IAnalyzable Compose(IAnalyzable[] source) =>
            IteratePairs(source, (left, right) => new Highest(left, right));

        private sealed class Highest : Composed
        {
            public Highest(IAnalyzable left, IAnalyzable right) : base(left, right) { }
            
            public override Outcome Evaluate() =>
                Outcome.Max(_left.Evaluate(), _right.Evaluate());

            public override RollProbabilityDistribution GetProbabilityDistribution() =>
                Expression.Transformation.SelectHighest(_left, _right).Evaluate();
        }
    }
}
