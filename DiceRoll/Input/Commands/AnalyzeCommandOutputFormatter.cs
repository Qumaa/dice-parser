namespace DiceRoll
{
    internal class AnalyzeCommandOutputFormatter : IAnalyzeCommandOutputFormatter
    {
        public string Rolling(Outcome outcome, Probability probability) =>
            $"Probability of rolling {outcome} is {probability}";

        public string Asserting(in Logical logical) =>
            $"Probability of {(logical.Outcome ? "succeeding" : "failing")} is {logical.Probability}";

        public string CumulativeFailing(Probability probability) =>
            $"Cumulative probability of failing is {probability}";

        public string CumulativeSucceeding(Probability probability) =>
            $"Cumulative probability of succeeding is {probability}";
    }
}
