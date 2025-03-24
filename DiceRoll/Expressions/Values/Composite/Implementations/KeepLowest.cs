namespace DiceRoll.Expressions
{
    public sealed class KeepLowest : Composer
    {
        protected override IAnalyzable Compose(IAnalyzable[] source) =>
            IteratePairs(source, (left, right) => new Lowest(left, right));

        private sealed class Lowest : Composed
        {
            public Lowest(IAnalyzable left, IAnalyzable right) : base(left, right) { }
            
            public override Outcome Evaluate()
            {
                Outcome left = _left.Evaluate();
                Outcome right = _right.Evaluate();

                return left.Value < right.Value ? left : right;
            }

            public override ProbabilityDistribution GetProbabilityDistribution() =>
                new Keep(_left.GetProbabilityDistribution(), _right.GetProbabilityDistribution(), KeepMode.Lowest)
                    .Evaluate();
        }
    }
}
