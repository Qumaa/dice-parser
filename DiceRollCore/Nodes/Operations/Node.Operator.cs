namespace DiceRoll
{
    public static partial class Node
    {
        /// <summary>
        /// Contains nodes that represent binary operations on numeric values.
        /// </summary>
        public static partial class Operator
        {
            /// <summary>
            /// Checks two arbitrary <see cref="INumeric">numeric nodes</see> to be equal.
            /// </summary>
            /// <param name="left">The first <see cref="INumeric">numeric node</see>.</param>
            /// <param name="right">The second <see cref="INumeric">numeric node</see>.</param>
            /// <returns>
            /// An <see cref="IAssertion">assertion</see> that checks two <see cref="INumeric">numeric nodes</see>.
            /// </returns>
            public static IOperation Equal(INumeric left, INumeric right) =>
                new DefaultOperation(left, right, OperationType.Equal);

            /// <summary>
            /// Checks two arbitrary <see cref="INumeric">numeric nodes</see> to not be equal.
            /// </summary>
            /// <param name="left">The first <see cref="INumeric">numeric node</see>.</param>
            /// <param name="right">The second <see cref="INumeric">numeric node</see>.</param>
            /// <returns>
            /// An <see cref="IAssertion">assertion</see> that checks two <see cref="INumeric">numeric nodes</see>.
            /// </returns>
            public static IOperation NotEqual(INumeric left, INumeric right) =>
                new DefaultOperation(left, right, OperationType.NotEqual);

            /// <summary>
            /// Checks two arbitrary <see cref="INumeric">numeric nodes</see> so that one is larger than the other.
            /// </summary>
            /// <param name="left">The first <see cref="INumeric">numeric node</see>.</param>
            /// <param name="right">The second <see cref="INumeric">numeric node</see>.</param>
            /// <returns>
            /// An <see cref="IAssertion">assertion</see> that checks two <see cref="INumeric">numeric nodes</see>.
            /// </returns>
            public static IOperation GreaterThan(INumeric left, INumeric right) =>
                new DefaultOperation(left, right, OperationType.GreaterThan);

            /// <summary>
            /// Checks two arbitrary <see cref="INumeric">numeric nodes</see> so that one is greater than or equal to
            /// the other.
            /// </summary>
            /// <param name="left">The first <see cref="INumeric">numeric node</see>.</param>
            /// <param name="right">The second <see cref="INumeric">numeric node</see>.</param>
            /// <returns>
            /// An <see cref="IAssertion">assertion</see> that checks two <see cref="INumeric">numeric nodes</see>.
            /// </returns>
            public static IOperation GreaterThanOrEqual(INumeric left, INumeric right) =>
                new DefaultOperation(left, right, OperationType.GreaterThanOrEqual);

            /// <summary>
            /// Checks two arbitrary <see cref="INumeric">numeric nodes</see> so that one is smaller than the other.
            /// </summary>
            /// <param name="left">The first <see cref="INumeric">numeric node</see>.</param>
            /// <param name="right">The second <see cref="INumeric">numeric node</see>.</param>
            /// <returns>
            /// An <see cref="IAssertion">assertion</see> that checks two <see cref="INumeric">numeric nodes</see>.
            /// </returns>
            public static IOperation LessThan(INumeric left, INumeric right) =>
                new DefaultOperation(left, right, OperationType.LessThan);

            /// <summary>
            /// Checks two arbitrary <see cref="INumeric">numeric nodes</see> so that one is smaller than or equal to
            /// the other.
            /// </summary>
            /// <param name="left">The first <see cref="INumeric">numeric node</see>.</param>
            /// <param name="right">The second <see cref="INumeric">numeric node</see>.</param>
            /// <returns>
            /// An <see cref="IAssertion">assertion</see> that checks two <see cref="INumeric">numeric nodes</see>.
            /// </returns>
            public static IOperation LessThanOrEqual(INumeric left, INumeric right) =>
                new DefaultOperation(left, right, OperationType.LessThanOrEqual);

            public static IAssertion And(IAssertion left, IAssertion right) =>
                new DefaultBinaryAssertion(left, right, BinaryAssertionType.And);
            
            public static IAssertion Or(IAssertion left, IAssertion right) =>
                new DefaultBinaryAssertion(left, right, BinaryAssertionType.Or);
            
            public static IAssertion Equal(IAssertion left, IAssertion right) =>
                new DefaultBinaryAssertion(left, right, BinaryAssertionType.Equal);
            
            public static IAssertion Not(IAssertion assertion) =>
                new NotAssertion(assertion);
        }
    }
}
