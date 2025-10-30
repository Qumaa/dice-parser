namespace DiceRoll.Input.Parsing
{
    public static class DiceOperand
    {
        public static readonly OperandDefinition Default = DefaultBuilder().Build();

        public static DiceOperandBuilder GetBuilder() =>
            new();

        public static DiceOperandBuilder DefaultBuilder() =>
            GetBuilder()
                .Delimiter(StringComparisonToken.CaseInsensitive("d"))
                .DefaultComposition(in CompositionDefinition.Summation)
                .Composition(in CompositionDefinition.Highest)
                .Composition(in CompositionDefinition.Lowest);
    }
}
