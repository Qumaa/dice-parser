namespace DiceRoll.Nodes
{
    public static partial class Node
    {
        public static partial class Operation
        {
            public static IOperation Equal(IAnalyzable left, IAnalyzable right) =>
                new DefaultOperation(left, right, OperationType.Equal);

            public static IOperation NotEqual(IAnalyzable left, IAnalyzable right) =>
                new DefaultOperation(left, right, OperationType.NotEqual);

            public static IOperation GreaterThan(IAnalyzable left, IAnalyzable right) =>
                new DefaultOperation(left, right, OperationType.GreaterThan);

            public static IOperation GreaterThanOrEqual(IAnalyzable left, IAnalyzable right) =>
                new DefaultOperation(left, right, OperationType.GreaterThanOrEqual);

            public static IOperation LessThan(IAnalyzable left, IAnalyzable right) =>
                new DefaultOperation(left, right, OperationType.LessThan);

            public static IOperation LessThanOrEqual(IAnalyzable left, IAnalyzable right) =>
                new DefaultOperation(left, right, OperationType.LessThanOrEqual);
        }
    }
}
