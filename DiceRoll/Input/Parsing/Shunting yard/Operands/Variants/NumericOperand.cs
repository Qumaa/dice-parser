namespace DiceRoll.Input.Parsing
{
    public static class NumericOperand
    {
        public static readonly OperandDefinition Default = BuildDefault();

        private static OperandDefinition BuildDefault() =>
            OperandDefinition.OfType<INumeric>(NumericToken.Shared, x => Node.Value.Constant(int.Parse(x.AsSpan())));
    }
}
