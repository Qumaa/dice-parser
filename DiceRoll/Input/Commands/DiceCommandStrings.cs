namespace DiceRoll
{
    public sealed class DiceCommandStrings
    {
        public readonly string RootDescription = "Enters the perpetual mode or evaluates the passed expression.";
        public readonly string RollDescription = "Evaluate the passed expression to one result.";
        public readonly string RollOutputFailedToPass = "Failed to pass.";
        public readonly string AnalyzeDescription =
            "Evaluate the passed expression to all possible results and specify their probabilities.";
        public readonly string DiceExpressionName = "Dice expression";
        public readonly string DiceExpressionDescription = "The expression to be evaluated.";

        public readonly IAnalyzeCommandOutputFormatter AnalyzeCommandOutput = new AnalyzeCommandOutputFormatter();
    }
}
