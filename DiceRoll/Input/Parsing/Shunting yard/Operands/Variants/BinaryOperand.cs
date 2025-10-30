namespace DiceRoll.Input.Parsing
{
    public static class BinaryOperand
    {
        public static readonly OperandDefinition Default = BuildDefault();

        private static OperandDefinition BuildDefault() =>
            OperandDefinition.OfType<IAssertion>(
                StringComparisonToken.CaseInsensitive("true", "false"),
                OperandParser.FromDelegate(x => Node.Value.Constant(bool.Parse(x.AsSpan())))
                );
    }
}
