using System;

namespace Dice.Expressions
{
    public static class Expression
    {
        public static Constant Constant(int value) =>
            new(value);

        public static Dice Dice(int faces, int number = 1) =>
            new(new Random(), faces, number);
        
        public static OperationExpression Equal(IAnalyzable left, IAnalyzable right) =>
            new(left, OperationType.Equal, right);
        public static OperationExpression NotEqual(IAnalyzable left, IAnalyzable right) =>
            new(left, OperationType.NotEqual, right);
        public static OperationExpression GreaterThan(IAnalyzable left, IAnalyzable right) =>
            new(left, OperationType.GreaterThan, right);
        public static OperationExpression GreaterThanOrEqual(IAnalyzable left, IAnalyzable right) =>
            new(left, OperationType.GreaterThanOrEqual, right);
        public static OperationExpression LessThan(IAnalyzable left, IAnalyzable right) =>
            new(left, OperationType.LessThan, right);
        public static OperationExpression LessThanOrEqual(IAnalyzable left, IAnalyzable right) =>
            new(left, OperationType.LessThanOrEqual, right);
    }
}
