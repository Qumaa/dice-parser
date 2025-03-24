namespace DiceRoll.Expressions
{
    public static partial class Expression
    {
        public static partial class Operations
        {
            public static Operation Equal(IAnalyzable left, IAnalyzable right) =>
                new(left, right, DefaultOperationDelegates.Get(OperationType.Equal));

            public static Operation NotEqual(IAnalyzable left, IAnalyzable right) =>
                new(left, right, DefaultOperationDelegates.Get(OperationType.NotEqual));

            public static Operation GreaterThan(IAnalyzable left, IAnalyzable right) =>
                new(left, right, DefaultOperationDelegates.Get(OperationType.GreaterThan));

            public static Operation GreaterThanOrEqual(IAnalyzable left, IAnalyzable right) =>
                new(left, right, DefaultOperationDelegates.Get(OperationType.GreaterThanOrEqual));

            public static Operation LessThan(IAnalyzable left, IAnalyzable right) =>
                new(left, right, DefaultOperationDelegates.Get(OperationType.LessThan));

            public static Operation LessThanOrEqual(IAnalyzable left, IAnalyzable right) =>
                new(left, right, DefaultOperationDelegates.Get(OperationType.LessThanOrEqual));
        }
    }
}
