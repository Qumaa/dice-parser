namespace DiceRoll.Input.Parsing
{
    public static class DiceOperand
    {
        public static readonly OperandDefinition Default = BuildDefault();

        public static DiceOperandBuilder StartBuilding() =>
            new();

        private static OperandDefinition BuildDefault() =>
            StartBuilding()
                .Delimiter(StringBasedToken.CaseInsensitive("d"))
                .DefaultComposition(in CompositionDefinition.Summation)
                .Composition(in CompositionDefinition.Highest)
                .Composition(in CompositionDefinition.Lowest)
                .Build();
    }
}
