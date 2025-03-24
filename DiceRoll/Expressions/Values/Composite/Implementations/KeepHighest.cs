namespace DiceRoll.Expressions
{
    public sealed class KeepHighest : Composer
    {
        protected override IAnalyzable Compose(IAnalyzable[] source) =>
            IteratePairs(source, (left, right) => new Highest(left, right));

        private sealed class Highest : Composed
        {
            public Highest(IAnalyzable left, IAnalyzable right) : base(left, right) { }
            
            public override Outcome Evaluate()
            {
                Outcome left = _left.Evaluate();
                Outcome right = _right.Evaluate();

                return left.Value > right.Value ? left : right;
            }

            public override ProbabilityDistribution GetProbabilityDistribution() =>
                new Keep(_left.GetProbabilityDistribution(), _right.GetProbabilityDistribution(), KeepMode.Highest)
                    .Evaluate();
        }
    }
}
