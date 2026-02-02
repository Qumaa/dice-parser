namespace DiceRoll
{
    public sealed class AnalyzeCommandStrings
    {
        public readonly string Description =
            "Evaluate the passed expression to all possible results and specify their probabilities";
        public readonly string StyleOptionDescription =
            "Specify the display format for relational expressions between two operands";
        public readonly string MaxWidthOptionDescription =
            "Limit the width of plots. 0, empty or negative stretches to fill the entire terminal";
        public readonly string Average = "Average";
        public readonly string StandardDeviation = "Deviation";
        public readonly string RollResultColumnHeader = "#";
        public readonly string ProbabilityColumnHeader = "%";
        
        public readonly IAnalyzeCommandOutputFormatter Formatter = new AnalyzeCommandOutputFormatter();
    }
}
