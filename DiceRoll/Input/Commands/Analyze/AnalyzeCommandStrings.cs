namespace DiceRoll
{
    public sealed class AnalyzeCommandStrings
    {
        public readonly string Description =
            "Evaluate the passed expression to all possible results and specify their probabilities";
        public readonly string MaxWidthOptionDescription =
            "Limit the width of plots. 0, empty or negative stretches to fill the entire terminal";
        public readonly string NormalizeOptionDescription =
            "Whether or not to scale the upper limit of plotted bars to the largest value or keep it fixed at 100 percent";
        public readonly string NormalizeOptionBehaviourDescription =
            "False unless the distribution is uniform";
        public readonly string Average = "Average";
        public readonly string StandardDeviation = "Deviation";
        public readonly string RollResultColumnHeader = "#";
        public readonly string ProbabilityColumnHeader = "%";
    }
}
