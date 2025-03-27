namespace DiceRoll.Nodes
{
    public static partial class Node
    {
        /// <summary>
        /// Contains nodes that represent numeric values and their probability distribution transformations.
        /// </summary>
        public static partial class Transformation
        {
            public static Combination Add(RollProbabilityDistribution left, RollProbabilityDistribution right) =>
                new(left, right, CombinationType.Add);
            public static Combination Add(IAnalyzable left, IAnalyzable right) =>
                Add(left.GetProbabilityDistribution(), right.GetProbabilityDistribution());

            public static Combination Subtract(RollProbabilityDistribution left, RollProbabilityDistribution right) =>
                new(left, right, CombinationType.Subtract);
            public static Combination Subtract(IAnalyzable left, IAnalyzable right) =>
                Subtract(left.GetProbabilityDistribution(), right.GetProbabilityDistribution());

            public static Selection SelectHighest(RollProbabilityDistribution left,
                RollProbabilityDistribution right) =>
                new(left, right, SelectionType.Highest);
            public static Selection SelectHighest(IAnalyzable left, IAnalyzable right) =>
                SelectHighest(left.GetProbabilityDistribution(), right.GetProbabilityDistribution());

            public static Selection SelectLowest(RollProbabilityDistribution left, RollProbabilityDistribution right) =>
                new(left, right, SelectionType.Lowest);
            public static Selection SelectLowest(IAnalyzable left, IAnalyzable right) =>
                SelectLowest(left.GetProbabilityDistribution(), right.GetProbabilityDistribution());

        }
    }
}
