namespace DiceRoll.FluentExtensions
{
    public static class NumericExtensions
    {
        public static INumeric AsConstant(this int value) =>
            Node.Value.Constant(value);

        public static INumeric AsDice(this int faces) =>
            Node.Value.Dice(faces);
        
        public static INumeric AsDice(this int dice, int faces) =>
            Node.Value.Dice(faces, dice);
        
        public static INumeric AsDice<T>(this int dice, int faces) where T : Composer, new() =>
            Node.Value.Dice<T>(faces, dice);
        
        public static INumeric AsDice(this int dice, Composer composer, int faces) =>
            Node.Value.Dice(composer, faces, dice);
        
        public static INumeric Add(this INumeric node, INumeric other) =>
            Node.Operator.Add(node, other);

        public static INumeric Subtract(this INumeric node, INumeric other) =>
            Node.Operator.Subtract(node, other);

        public static INumeric Multiply(this INumeric node, INumeric multiplier) =>
            Node.Operator.Multiply(node, multiplier);

        public static INumeric DivideRoundDown(this INumeric dividend, INumeric divisor) =>
            Node.Operator.DivideRoundDown(dividend, divisor);

        public static INumeric DivideRoundUp(this INumeric dividend, INumeric divisor) =>
            Node.Operator.DivideRoundUp(dividend, divisor);

        public static INumeric SelectHighest(this INumeric node, INumeric other) =>
            Node.Operator.SelectHighest(node, other);

        public static INumeric SelectLowest(this INumeric node, INumeric other) =>
            Node.Operator.SelectLowest(node, other);

        public static INumeric Negate(this INumeric node) =>
            Node.Operator.Negate(node);
        
        public static IOperation Equal(this INumeric node, INumeric other) =>
            Node.Operator.Equal(node, other);

        public static IOperation NotEqual(this INumeric node, INumeric other) =>
            Node.Operator.NotEqual(node, other);

        public static IOperation GreaterThan(this INumeric node, INumeric other) =>
            Node.Operator.GreaterThan(node, other);

        public static IOperation GreaterThanOrEqual(this INumeric node, INumeric other) =>
            Node.Operator.GreaterThanOrEqual(node, other);

        public static IOperation LessThan(this INumeric node, INumeric other) =>
            Node.Operator.LessThan(node, other);

        public static IOperation LessThanOrEqual(this INumeric node, INumeric other) =>
            Node.Operator.LessThanOrEqual(node, other);
    }
}
