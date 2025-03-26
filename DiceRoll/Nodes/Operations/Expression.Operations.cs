namespace DiceRoll.Nodes
{
    public static partial class Node
    {
        public static partial class Operation
        {
            public static Nodes.Operation Equal(IAnalyzable left, IAnalyzable right) =>
                new(left, right, DefaultOperationDelegates.Get(OperationType.Equal));

            public static Nodes.Operation NotEqual(IAnalyzable left, IAnalyzable right) =>
                new(left, right, DefaultOperationDelegates.Get(OperationType.NotEqual));

            public static Nodes.Operation GreaterThan(IAnalyzable left, IAnalyzable right) =>
                new(left, right, DefaultOperationDelegates.Get(OperationType.GreaterThan));

            public static Nodes.Operation GreaterThanOrEqual(IAnalyzable left, IAnalyzable right) =>
                new(left, right, DefaultOperationDelegates.Get(OperationType.GreaterThanOrEqual));

            public static Nodes.Operation LessThan(IAnalyzable left, IAnalyzable right) =>
                new(left, right, DefaultOperationDelegates.Get(OperationType.LessThan));

            public static Nodes.Operation LessThanOrEqual(IAnalyzable left, IAnalyzable right) =>
                new(left, right, DefaultOperationDelegates.Get(OperationType.LessThanOrEqual));
        }
    }
}
