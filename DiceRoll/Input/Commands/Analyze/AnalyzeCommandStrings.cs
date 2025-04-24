namespace DiceRoll
{
    public sealed class AnalyzeCommandStrings
    {
        public readonly string Description =
            "Evaluate the passed expression to all possible results and specify their probabilities";
        public readonly string StyleOptionDescription =
            "Specify the display format for relational expressions between two operands";
        
        public readonly IAnalyzeCommandOutputFormatter AnalyzeCommandOutput = new AnalyzeCommandOutputFormatter();
    }
}
