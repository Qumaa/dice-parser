namespace DiceRoll.Nodes
{
    public static partial class Node
    {
        /// <summary>
        /// Contains nodes that represent numeric values and their probability distribution transformations.
        /// </summary>
        public static partial class Transformation
        {
            public static IAnalyzable Add(IAnalyzable left, IAnalyzable right) =>
                new Combination(left, right, CombinationType.Add);

            public static IAnalyzable Subtract(IAnalyzable left, IAnalyzable right) =>
                new Combination(left, right, CombinationType.Subtract);

            public static IAnalyzable SelectHighest(IAnalyzable left, IAnalyzable right) =>
                new Selection(left, right, SelectionType.Highest);

            public static IAnalyzable SelectLowest(IAnalyzable left, IAnalyzable right) =>
                new Selection(left, right, SelectionType.Lowest);

        }
    }
}
