namespace DiceRoll.Input.Parsing
{
    public enum OperatorKind
    {
        Unary,
        Binary
    }

    public static class OperatorKindExtensions
    {
        public static OperatorKind Reversed(this OperatorKind operatorKind) =>
            operatorKind switch
            {
                OperatorKind.Unary => OperatorKind.Binary,
                OperatorKind.Binary => OperatorKind.Unary
            };
    }
}
