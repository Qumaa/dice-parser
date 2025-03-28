namespace DiceRoll.Nodes
{
    public static partial class Node
    {
        public static partial class Operation
        {
            public static IOperation Equal(IAnalyzable left, IAnalyzable right) =>
                new Nodes.Operation(left, right, DefaultOperationDelegates.Get(OperationType.Equal));

            public static IOperation NotEqual(IAnalyzable left, IAnalyzable right) =>
                new Nodes.Operation(left, right, DefaultOperationDelegates.Get(OperationType.NotEqual));

            public static IOperation GreaterThan(IAnalyzable left, IAnalyzable right) =>
                new Nodes.Operation(left, right, DefaultOperationDelegates.Get(OperationType.GreaterThan));

            public static IOperation GreaterThanOrEqual(IAnalyzable left, IAnalyzable right) =>
                new Nodes.Operation(left, right, DefaultOperationDelegates.Get(OperationType.GreaterThanOrEqual));

            public static IOperation LessThan(IAnalyzable left, IAnalyzable right) =>
                new Nodes.Operation(left, right, DefaultOperationDelegates.Get(OperationType.LessThan));

            public static IOperation LessThanOrEqual(IAnalyzable left, IAnalyzable right) =>
                new Nodes.Operation(left, right, DefaultOperationDelegates.Get(OperationType.LessThanOrEqual));
        }
    }
}
