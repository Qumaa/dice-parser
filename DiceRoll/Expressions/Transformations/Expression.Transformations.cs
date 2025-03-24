namespace DiceRoll.Expressions
{
    public static partial class Expression
    {
        public static partial class Transformations
        {
            public static Combination Add(ProbabilityDistribution left, ProbabilityDistribution right) =>
                new(left, right, CombinationType.Add);
            public static Combination Add(IAnalyzable left, IAnalyzable right) =>
                Add(left.GetProbabilityDistribution(), right.GetProbabilityDistribution());
            public static Combination Add(IAnalyzable left, ProbabilityDistribution right) =>
                Add(left.GetProbabilityDistribution(), right);
            public static Combination Add(ProbabilityDistribution left, IAnalyzable right) =>
                Add(left, right.GetProbabilityDistribution());

            public static Combination Subtract(ProbabilityDistribution left, ProbabilityDistribution right) =>
                new(left, right, CombinationType.Subtract);
            public static Combination Subtract(IAnalyzable left, IAnalyzable right) =>
                Subtract(left.GetProbabilityDistribution(), right.GetProbabilityDistribution());
            public static Combination Subtract(IAnalyzable left, ProbabilityDistribution right) =>
                Subtract(left.GetProbabilityDistribution(), right);
            public static Combination Subtract(ProbabilityDistribution left, IAnalyzable right) =>
                Subtract(left, right.GetProbabilityDistribution());
        }
    }
}
