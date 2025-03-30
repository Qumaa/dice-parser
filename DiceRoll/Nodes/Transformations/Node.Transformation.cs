namespace DiceRoll.Nodes
{
    public static partial class Node
    {
        /// <summary>
        /// Contains nodes that represent transformations of numeric values and their probability distribution.
        /// </summary>
        public static partial class Transformation
        {
            /// <summary>
            /// Adds two arbitrary <see cref="IAnalyzable">numeric nodes</see>
            /// and provides an updated <see cref="RollProbabilityDistribution">probability distribution</see>
            /// of the results.
            /// </summary>
            /// <param name="left">The first <see cref="IAnalyzable">numeric node</see>.</param>
            /// <param name="right">The second <see cref="IAnalyzable">numeric node</see>.</param>
            /// <returns>
            /// A <see cref="IAnalyzable">numeric node</see> that adds
            /// <paramref name="left"/> and <paramref name="right"/>
            /// and provides an updated <see cref="RollProbabilityDistribution">probability distribution</see>.
            /// </returns>
            public static IAnalyzable Add(IAnalyzable left, IAnalyzable right) =>
                new Combination(left, right, CombinationType.Add);

            /// <summary>
            /// Subtracts one arbitrary <see cref="IAnalyzable">numeric node</see> from another
            /// and provides an updated <see cref="RollProbabilityDistribution">probability distribution</see>
            /// of the results.
            /// </summary>
            /// <param name="left">The first <see cref="IAnalyzable">numeric node</see>.</param>
            /// <param name="right">The second <see cref="IAnalyzable">numeric node</see>.</param>
            /// <returns>
            /// A <see cref="IAnalyzable">numeric node</see> that subtracts
            /// <paramref name="right"/> from <paramref name="left"/>
            /// and provides an updated <see cref="RollProbabilityDistribution">probability distribution</see>.
            /// </returns>
            public static IAnalyzable Subtract(IAnalyzable left, IAnalyzable right) =>
                new Combination(left, right, CombinationType.Subtract);
            
            public static IAnalyzable Multiply(IAnalyzable left, IAnalyzable right) =>
                new Combination(left, right, CombinationType.Multiply);
            
            public static IAnalyzable DivideRoundDown(IAnalyzable left, IAnalyzable right) =>
                new Combination(left, right, CombinationType.DivideRoundDownwards);
            
            public static IAnalyzable DivideRoundUp(IAnalyzable left, IAnalyzable right) =>
                new Combination(left, right, CombinationType.DivideRoundUpwards);

            /// <summary>
            /// Selects the highest evaluation result of two arbitrary <see cref="IAnalyzable">numeric nodes</see>
            /// and provides an updated <see cref="RollProbabilityDistribution">probability distribution</see>
            /// of the results.
            /// </summary>
            /// <param name="left">The first <see cref="IAnalyzable">numeric node</see>.</param>
            /// <param name="right">The second <see cref="IAnalyzable">numeric node</see>.</param>
            /// <returns>
            /// A <see cref="IAnalyzable">numeric node</see> that selects the highest evaluation result of
            /// <paramref name="left"/> and <paramref name="right"/>
            /// and provides an updated <see cref="RollProbabilityDistribution">probability distribution</see>.
            /// </returns>
            public static IAnalyzable SelectHighest(IAnalyzable left, IAnalyzable right) =>
                new Selection(left, right, SelectionType.Highest);

            /// <summary>
            /// Selects the lowest evaluation result of two arbitrary <see cref="IAnalyzable">numeric nodes</see>
            /// and provides an updated <see cref="RollProbabilityDistribution">probability distribution</see>
            /// of the results.
            /// </summary>
            /// <param name="left">The first <see cref="IAnalyzable">numeric node</see>.</param>
            /// <param name="right">The second <see cref="IAnalyzable">numeric node</see>.</param>
            /// <returns>
            /// A <see cref="IAnalyzable">numeric node</see> that selects the lowest evaluation result of
            /// <paramref name="left"/> and <paramref name="right"/>
            /// and provides an updated <see cref="RollProbabilityDistribution">probability distribution</see>.
            /// </returns>
            public static IAnalyzable SelectLowest(IAnalyzable left, IAnalyzable right) =>
                new Selection(left, right, SelectionType.Lowest);

        }
    }
}
