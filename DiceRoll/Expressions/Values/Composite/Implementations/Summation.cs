namespace DiceRoll.Expressions
{
    public sealed class Summation : Composer
    {
        protected override IAnalyzable Compose(IAnalyzable[] source) =>
            IteratePairs(source, (left, right) => new Sum(left, right));

        private sealed class Sum : Composed
        {
            public Sum(IAnalyzable left, IAnalyzable right) : base(left, right) { }

            public override Outcome Evaluate() =>
                new(_left.Evaluate().Value + _right.Evaluate().Value);

            public override RollProbabilityDistribution GetProbabilityDistribution() =>
                Expression.Transformation.Add(_left, _right).Evaluate();
        }
    }
}
