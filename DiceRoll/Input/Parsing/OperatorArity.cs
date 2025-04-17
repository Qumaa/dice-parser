namespace DiceRoll.Input.Parsing
{
    public enum OperatorArity
    {
        Unary,
        Binary
    }

    public static class OperatorKindExtensions
    {
        public static OperatorArity Reversed(this OperatorArity operatorArity) =>
            operatorArity switch
            {
                OperatorArity.Unary => OperatorArity.Binary,
                OperatorArity.Binary => OperatorArity.Unary
            };
    }
}
