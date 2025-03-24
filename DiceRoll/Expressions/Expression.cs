using System;

namespace DiceRoll.Expressions
{
    public static class Expression
    {
        public static Constant Constant(int value) =>
            new(value);

        public static Dice Dice(int faces) =>
            new(new Random(), faces);
        
        public static Operation Equal(IAnalyzable left, IAnalyzable right) =>
            new(left, DefaultOperationDelegates.Get(OperationType.Equal), right);
        public static Operation NotEqual(IAnalyzable left, IAnalyzable right) =>
            new(left, DefaultOperationDelegates.Get(OperationType.NotEqual), right);
        public static Operation GreaterThan(IAnalyzable left, IAnalyzable right) =>
            new(left, DefaultOperationDelegates.Get(OperationType.GreaterThan), right);
        public static Operation GreaterThanOrEqual(IAnalyzable left, IAnalyzable right) =>
            new(left, DefaultOperationDelegates.Get(OperationType.GreaterThanOrEqual), right);
        public static Operation LessThan(IAnalyzable left, IAnalyzable right) =>
            new(left, DefaultOperationDelegates.Get(OperationType.LessThan), right);
        public static Operation LessThanOrEqual(IAnalyzable left, IAnalyzable right) =>
            new(left, DefaultOperationDelegates.Get(OperationType.LessThanOrEqual), right);
    }
}
