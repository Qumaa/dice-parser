namespace DiceRoll
{
    public static partial class Node
    {
        /// <summary>
        /// Contains nodes that represent binary operations on numeric values.
        /// </summary>
        public static partial class Operation
        {
            /// <summary>
            /// Checks two arbitrary <see cref="IAnalyzable">numeric nodes</see> to be equal.
            /// </summary>
            /// <param name="left">The first <see cref="IAnalyzable">numeric node</see>.</param>
            /// <param name="right">The second <see cref="IAnalyzable">numeric node</see>.</param>
            /// <returns>
            /// An <see cref="IOperation">operation</see> that checks two <see cref="IAnalyzable">numeric nodes</see>.
            /// </returns>
            public static IOperation Equal(IAnalyzable left, IAnalyzable right) =>
                new DefaultNumericOperation(left, right, NumericOperationType.Equal);

            /// <summary>
            /// Checks two arbitrary <see cref="IAnalyzable">numeric nodes</see> to not be equal.
            /// </summary>
            /// <param name="left">The first <see cref="IAnalyzable">numeric node</see>.</param>
            /// <param name="right">The second <see cref="IAnalyzable">numeric node</see>.</param>
            /// <returns>
            /// An <see cref="IOperation">operation</see> that checks two <see cref="IAnalyzable">numeric nodes</see>.
            /// </returns>
            public static IOperation NotEqual(IAnalyzable left, IAnalyzable right) =>
                new DefaultNumericOperation(left, right, NumericOperationType.NotEqual);

            /// <summary>
            /// Checks two arbitrary <see cref="IAnalyzable">numeric nodes</see> so that one is larger than the other.
            /// </summary>
            /// <param name="left">The first <see cref="IAnalyzable">numeric node</see>.</param>
            /// <param name="right">The second <see cref="IAnalyzable">numeric node</see>.</param>
            /// <returns>
            /// An <see cref="IOperation">operation</see> that checks two <see cref="IAnalyzable">numeric nodes</see>.
            /// </returns>
            public static IOperation GreaterThan(IAnalyzable left, IAnalyzable right) =>
                new DefaultNumericOperation(left, right, NumericOperationType.GreaterThan);

            /// <summary>
            /// Checks two arbitrary <see cref="IAnalyzable">numeric nodes</see> so that one is greater than or equal to
            /// the other.
            /// </summary>
            /// <param name="left">The first <see cref="IAnalyzable">numeric node</see>.</param>
            /// <param name="right">The second <see cref="IAnalyzable">numeric node</see>.</param>
            /// <returns>
            /// An <see cref="IOperation">operation</see> that checks two <see cref="IAnalyzable">numeric nodes</see>.
            /// </returns>
            public static IOperation GreaterThanOrEqual(IAnalyzable left, IAnalyzable right) =>
                new DefaultNumericOperation(left, right, NumericOperationType.GreaterThanOrEqual);

            /// <summary>
            /// Checks two arbitrary <see cref="IAnalyzable">numeric nodes</see> so that one is smaller than the other.
            /// </summary>
            /// <param name="left">The first <see cref="IAnalyzable">numeric node</see>.</param>
            /// <param name="right">The second <see cref="IAnalyzable">numeric node</see>.</param>
            /// <returns>
            /// An <see cref="IOperation">operation</see> that checks two <see cref="IAnalyzable">numeric nodes</see>.
            /// </returns>
            public static IOperation LessThan(IAnalyzable left, IAnalyzable right) =>
                new DefaultNumericOperation(left, right, NumericOperationType.LessThan);

            /// <summary>
            /// Checks two arbitrary <see cref="IAnalyzable">numeric nodes</see> so that one is smaller than or equal to
            /// the other.
            /// </summary>
            /// <param name="left">The first <see cref="IAnalyzable">numeric node</see>.</param>
            /// <param name="right">The second <see cref="IAnalyzable">numeric node</see>.</param>
            /// <returns>
            /// An <see cref="IOperation">operation</see> that checks two <see cref="IAnalyzable">numeric nodes</see>.
            /// </returns>
            public static IOperation LessThanOrEqual(IAnalyzable left, IAnalyzable right) =>
                new DefaultNumericOperation(left, right, NumericOperationType.LessThanOrEqual);

            public static IOperation And(IOperation left, IOperation right) =>
                new DefaultBinaryOperation(left, right, BinaryOperationType.And);
            
            public static IOperation Or(IOperation left, IOperation right) =>
                new DefaultBinaryOperation(left, right, BinaryOperationType.Or);
            
            public static IOperation Equal(IOperation left, IOperation right) =>
                new DefaultBinaryOperation(left, right, BinaryOperationType.Equal);
            
            public static IOperation Not(IOperation operation) =>
                new NotBinaryOperation(operation);
        }
    }
}
