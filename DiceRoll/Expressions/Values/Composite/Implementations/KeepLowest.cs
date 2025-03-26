namespace DiceRoll.Expressions
{
    public sealed class KeepLowest : Composer
    {
        protected override IAnalyzable Compose(IAnalyzable[] source) =>
            IteratePairs(source, (left, right) => new Lowest(left, right));

        private sealed class Lowest : Composed
        {
            public Lowest(IAnalyzable left, IAnalyzable right) : base(left, right) { }
            
            public override Outcome Evaluate() =>
                Outcome.Min(_left.Evaluate(), _right.Evaluate());

            public override RollProbabilityDistribution GetProbabilityDistribution() =>
                Expression.Transformation.SelectLowest(_left, _right).Evaluate();
        }
    }
}
