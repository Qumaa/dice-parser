namespace DiceRoll.Input.Parsing
{
    public enum OperatorUsageForm
    {
        /// <summary>
        /// Operator is in between its arguments. Used when a previously parsed token is an operand.
        /// </summary>
        Infix,
        /// <summary>
        /// Operator is in front of its arguments. Used when a previously parsed token is also an operator.
        /// </summary>
        Prefix
    }
}
