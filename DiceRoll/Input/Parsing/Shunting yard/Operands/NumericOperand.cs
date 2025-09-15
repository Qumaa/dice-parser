namespace DiceRoll.Input.Parsing
{
    public static class NumericOperand
    {
        public static readonly OperandDefinition Default = BuildDefault();

        private static OperandDefinition BuildDefault() =>
            new(NumericToken.Shared, x => Node.Value.Constant(int.Parse(x.AsSpan())));
    }
}
