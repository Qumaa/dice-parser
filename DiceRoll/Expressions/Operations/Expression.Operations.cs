namespace DiceRoll.Expressions
{
    public static partial class Expression
    {
        public static partial class Operation
        {
            public static Expressions.Operation Equal(IAnalyzable left, IAnalyzable right) =>
                new(left, right, DefaultOperationDelegates.Get(OperationType.Equal));

            public static Expressions.Operation NotEqual(IAnalyzable left, IAnalyzable right) =>
                new(left, right, DefaultOperationDelegates.Get(OperationType.NotEqual));

            public static Expressions.Operation GreaterThan(IAnalyzable left, IAnalyzable right) =>
                new(left, right, DefaultOperationDelegates.Get(OperationType.GreaterThan));

            public static Expressions.Operation GreaterThanOrEqual(IAnalyzable left, IAnalyzable right) =>
                new(left, right, DefaultOperationDelegates.Get(OperationType.GreaterThanOrEqual));

            public static Expressions.Operation LessThan(IAnalyzable left, IAnalyzable right) =>
                new(left, right, DefaultOperationDelegates.Get(OperationType.LessThan));

            public static Expressions.Operation LessThanOrEqual(IAnalyzable left, IAnalyzable right) =>
                new(left, right, DefaultOperationDelegates.Get(OperationType.LessThanOrEqual));
        }
    }
}
