namespace DiceRoll
{
    public static partial class Node
    {
        public static partial class Operator
        {
            public static INumeric Add(INumeric left, INumeric right) =>
                new Combination(left, right, CombinationType.Add);

            public static INumeric Subtract(INumeric left, INumeric right) =>
                new Combination(left, right, CombinationType.Subtract);

            public static INumeric Multiply(INumeric left, INumeric right) =>
                new Combination(left, right, CombinationType.Multiply);

            public static INumeric DivideRoundDown(INumeric left, INumeric right) =>
                new Combination(left, right, CombinationType.DivideRoundDownwards);

            public static INumeric DivideRoundUp(INumeric left, INumeric right) =>
                new Combination(left, right, CombinationType.DivideRoundUpwards);

            public static INumeric SelectHighest(INumeric left, INumeric right) =>
                new Selection(left, right, SelectionType.Highest);

            public static INumeric SelectLowest(INumeric left, INumeric right) =>
                new Selection(left, right, SelectionType.Lowest);

            public static INumeric Negate(INumeric node) =>
                new Negation(node);
        }
    }
}
