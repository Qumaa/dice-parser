namespace DiceRoll.Input.Parsing
{
    public enum OperatorUsageForm
    {
        /// <summary>
        /// Operator is in between its arguments
        /// </summary>
        Infix,
        /// <summary>
        /// Operator is in front of its arguments
        /// </summary>
        Prefix
    }

    public static class OperatorKindExtensions
    {
        public static OperatorUsageForm Opposite(this OperatorUsageForm operatorUsageForm) =>
            operatorUsageForm switch
            {
                OperatorUsageForm.Infix => OperatorUsageForm.Prefix,
                OperatorUsageForm.Prefix => OperatorUsageForm.Infix,
                _ => throw new EnumValueNotDefinedException<OperatorUsageForm>(nameof(operatorUsageForm))
            };
    }
}
