namespace DiceRoll
{
    internal class AnalyzeCommandOutputFormatter : IAnalyzeCommandOutputFormatter
    {
        public string Rolling(string outcome, string probability, string bar) =>
            $"{outcome} | {bar} {probability}";

        public string Asserting(in Logical logical) =>
            $"Probability of {(logical.Outcome ? "succeeding" : "failing")} is {logical.Probability}";

        public string CumulativeFailing(Probability probability) =>
            $"Cumulative probability of failing is {probability}";

        public string CumulativeSucceeding(Probability probability) =>
            $"Cumulative probability of succeeding is {probability}";
    }
}
