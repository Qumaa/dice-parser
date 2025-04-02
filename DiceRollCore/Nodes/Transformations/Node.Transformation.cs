namespace DiceRoll
{
    public static partial class Node
    {
        /// <summary>
        /// Contains nodes that represent transformations of numeric values and their probability distribution.
        /// </summary>
        public static partial class Transformation
        {
            /// <summary>
            /// Adds two arbitrary <see cref="INumeric">numeric nodes</see>
            /// and provides an updated <see cref="RollProbabilityDistribution">probability distribution</see>
            /// of the results.
            /// </summary>
            /// <param name="left">The first <see cref="INumeric">numeric node</see>.</param>
            /// <param name="right">The second <see cref="INumeric">numeric node</see>.</param>
            /// <returns>
            /// A <see cref="INumeric">numeric node</see> that adds
            /// <paramref name="left"/> and <paramref name="right"/>
            /// and provides an updated <see cref="RollProbabilityDistribution">probability distribution</see>.
            /// </returns>
            public static INumeric Add(INumeric left, INumeric right) =>
                new Combination(left, right, CombinationType.Add);

            /// <summary>
            /// Subtracts one arbitrary <see cref="INumeric">numeric node</see> from another
            /// and provides an updated <see cref="RollProbabilityDistribution">probability distribution</see>
            /// of the results.
            /// </summary>
            /// <param name="left">The first <see cref="INumeric">numeric node</see>.</param>
            /// <param name="right">The second <see cref="INumeric">numeric node</see>.</param>
            /// <returns>
            /// A <see cref="INumeric">numeric node</see> that subtracts
            /// <paramref name="right"/> from <paramref name="left"/>
            /// and provides an updated <see cref="RollProbabilityDistribution">probability distribution</see>.
            /// </returns>
            public static INumeric Subtract(INumeric left, INumeric right) =>
                new Combination(left, right, CombinationType.Subtract);
            
            public static INumeric Multiply(INumeric left, INumeric right) =>
                new Combination(left, right, CombinationType.Multiply);
            
            public static INumeric DivideRoundDown(INumeric left, INumeric right) =>
                new Combination(left, right, CombinationType.DivideRoundDownwards);
            
            public static INumeric DivideRoundUp(INumeric left, INumeric right) =>
                new Combination(left, right, CombinationType.DivideRoundUpwards);

            /// <summary>
            /// Selects the highest evaluation result of two arbitrary <see cref="INumeric">numeric nodes</see>
            /// and provides an updated <see cref="RollProbabilityDistribution">probability distribution</see>
            /// of the results.
            /// </summary>
            /// <param name="left">The first <see cref="INumeric">numeric node</see>.</param>
            /// <param name="right">The second <see cref="INumeric">numeric node</see>.</param>
            /// <returns>
            /// A <see cref="INumeric">numeric node</see> that selects the highest evaluation result of
            /// <paramref name="left"/> and <paramref name="right"/>
            /// and provides an updated <see cref="RollProbabilityDistribution">probability distribution</see>.
            /// </returns>
            public static INumeric SelectHighest(INumeric left, INumeric right) =>
                new Selection(left, right, SelectionType.Highest);

            /// <summary>
            /// Selects the lowest evaluation result of two arbitrary <see cref="INumeric">numeric nodes</see>
            /// and provides an updated <see cref="RollProbabilityDistribution">probability distribution</see>
            /// of the results.
            /// </summary>
            /// <param name="left">The first <see cref="INumeric">numeric node</see>.</param>
            /// <param name="right">The second <see cref="INumeric">numeric node</see>.</param>
            /// <returns>
            /// A <see cref="INumeric">numeric node</see> that selects the lowest evaluation result of
            /// <paramref name="left"/> and <paramref name="right"/>
            /// and provides an updated <see cref="RollProbabilityDistribution">probability distribution</see>.
            /// </returns>
            public static INumeric SelectLowest(INumeric left, INumeric right) =>
                new Selection(left, right, SelectionType.Lowest);

        }
    }
}
