namespace DiceRoll
{
    public static partial class Node
    {
        public static partial class Operator
        {
            public static IOperation Equal(INumeric left, INumeric right) =>
                new DefaultBinaryOperation(left, right, OperationType.Equal);

            public static IOperation NotEqual(INumeric left, INumeric right) =>
                new DefaultBinaryOperation(left, right, OperationType.NotEqual);

            public static IOperation GreaterThan(INumeric left, INumeric right) =>
                new DefaultBinaryOperation(left, right, OperationType.GreaterThan);

            public static IOperation GreaterThanOrEqual(INumeric left, INumeric right) =>
                new DefaultBinaryOperation(left, right, OperationType.GreaterThanOrEqual);

            public static IOperation LessThan(INumeric left, INumeric right) =>
                new DefaultBinaryOperation(left, right, OperationType.LessThan);

            public static IOperation LessThanOrEqual(INumeric left, INumeric right) =>
                new DefaultBinaryOperation(left, right, OperationType.LessThanOrEqual);

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
