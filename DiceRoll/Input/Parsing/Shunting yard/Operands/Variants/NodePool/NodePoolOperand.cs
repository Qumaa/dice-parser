namespace DiceRoll.Input.Parsing
{
    public static class NodePoolOperand
    {
        public static readonly OperandDefinition Default = BuildDefault().Build();

        public static NodePoolOperandBuilder GetBuilder() =>
            new();

        public static NodePoolOperandBuilder BuildDefault() =>
            GetBuilder()
                .OpenScope(StringComparisonToken.CaseInsensitive("(", "[", "{"))
                .CloseScope(StringComparisonToken.CaseInsensitive(")", "]", "}"))
                .Separator(StringComparisonToken.CaseInsensitive(",", ";"));
    }
}
