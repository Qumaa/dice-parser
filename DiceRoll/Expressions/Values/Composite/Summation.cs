namespace DiceRoll.Expressions
{
    public sealed class Summation : Composer
    {
        protected override IAnalyzable Compose(IAnalyzable[] source)
        {
            for (int i = 1; i < source.Length; i++)
            {
                IAnalyzable previous = source[i - 1];
                IAnalyzable current = source[i];

                source[i] = new Composed(previous, current);
            }

            return source[^1];
        }

        private sealed class Composed : IAnalyzable
        {
            private readonly IAnalyzable _left;
            private readonly IAnalyzable _right;
            
            public Composed(IAnalyzable left, IAnalyzable right)
            {
                _left = left;
                _right = right;
            }

            public Outcome Evaluate() =>
                new(_left.Evaluate().Value + _right.Evaluate().Value);

            public ProbabilityDistribution GetProbabilityDistribution() =>
                Expression.Transformations.Add(_left, _right).Evaluate();
        }
    }
}
