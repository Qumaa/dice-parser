namespace DiceRoll
{
    public sealed class DiceCommandStrings
    {
        public readonly string RootDescription = "Enter the perpetual mode or evaluate the passed expression";
        public readonly string RollDescription = "Evaluate the passed expression to one result";
        public readonly string RollOutputFailedToPass = "Failed to pass";
        public readonly string AnalyzeDescription =
            "Evaluate the passed expression to all possible results and specify their probabilities";
        public readonly string AnalyzeStyleOptionDescription =
            "Specify the display format for relational expressions between two operands";
        public readonly string DiceExpressionName = "Dice expression";
        public readonly string DiceExpressionDescription = "The expression to be evaluated";
        public readonly string WIP = "This functionality is under development.";

        public readonly IAnalyzeCommandOutputFormatter AnalyzeCommandOutput = new AnalyzeCommandOutputFormatter();
    }
}
