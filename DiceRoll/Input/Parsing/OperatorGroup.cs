namespace DiceRoll.Input.Parsing
{
    public enum OperatorGroup
    {
        LeftSideArguments,
        RightSideArguments
    }

    public static class OperatorKindExtensions
    {
        public static OperatorGroup Reversed(this OperatorGroup operatorGroup) =>
            operatorGroup switch
            {
                OperatorGroup.LeftSideArguments => OperatorGroup.RightSideArguments,
                OperatorGroup.RightSideArguments => OperatorGroup.LeftSideArguments,
                _ => throw new EnumValueNotDefinedException<OperatorGroup>(nameof(operatorGroup))
            };
    }
}
