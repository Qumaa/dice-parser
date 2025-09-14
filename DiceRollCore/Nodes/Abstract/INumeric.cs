namespace DiceRoll
{
    public interface INumeric : INode<Outcome>, IDistributable<RollProbabilityDistribution, Roll> { }

    public static class NumericExtensions
    {
        public static INumeric Add(this INumeric node, INumeric other) =>
            Node.Operator.Add(node, other);

        public static INumeric Subtract(this INumeric node, INumeric other) =>
            Node.Operator.Subtract(node, other);

        public static INumeric Multiply(this INumeric node, INumeric other) =>
            Node.Operator.Multiply(node, other);

        public static INumeric DivideRoundDown(this INumeric node, INumeric other) =>
            Node.Operator.DivideRoundDown(node, other);

        public static INumeric DivideRoundUp(this INumeric node, INumeric other) =>
            Node.Operator.DivideRoundUp(node, other);

        public static INumeric SelectHighest(this INumeric node, INumeric other) =>
            Node.Operator.SelectHighest(node, other);

        public static INumeric SelectLowest(this INumeric node, INumeric other) =>
            Node.Operator.SelectLowest(node, other);

        public static INumeric Negate(this INumeric node) =>
            Node.Operator.Negate(node);
        
        public static INumeric Composite(this INumeric node, Composer composer, INumeric other,
            params INumeric[] sequence) =>
            Node.Value.Composite(composer, node, other, sequence);
            
        public static INumeric Composite(this INumeric node, Composer composer, int repetitionCount) =>
            Node.Value.Composite(composer, node, repetitionCount);

        public static INumeric Composite<T>(this INumeric node, INumeric other, params INumeric[] sequence)
            where T : Composer, new() =>
            Node.Value.Composite<T>(node, other, sequence);
            
        public static INumeric Composite<T>(this INumeric node, int repetitionCount)
            where T : Composer, new() =>
            Node.Value.Composite<T>(node, repetitionCount);

        public static INumeric Summation(this INumeric node, INumeric other, params INumeric[] sequence) =>
            Node.Value.Summation(node, other, sequence);
            
        public static INumeric Summation(this INumeric node, int repetitionCount) =>
            Node.Value.Summation(node, repetitionCount);

        public static INumeric Highest(this INumeric node, INumeric other, params INumeric[] sequence) =>
            Node.Value.Highest(node, other, sequence);
            
        public static INumeric Highest(this INumeric node, int repetitionCount) =>
            Node.Value.Highest(node, repetitionCount);

        public static INumeric Lowest(this INumeric node, INumeric other, params INumeric[] sequence) =>
            Node.Value.Lowest(node, other, sequence);
            
        public static INumeric Lowest(this INumeric node, int repetitionCount) =>
            Node.Value.Lowest(node, repetitionCount);
        
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
