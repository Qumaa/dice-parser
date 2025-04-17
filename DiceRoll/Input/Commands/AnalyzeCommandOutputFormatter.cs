namespace DiceRoll
{
    internal class AnalyzeCommandOutputFormatter : IAnalyzeCommandOutputFormatter
    {
        public string Rolling(Outcome outcome, Probability probability) =>
            $"Probability of {outcome} is {probability}";

        public string Asserting(in Logical logical) =>
            $"Probability of {logical.Outcome} is {logical.Probability}";

        public string Failing(Probability probability) =>
            $"Cumulative probability of failing is {probability}";

        public string Succeeding(Probability probability) =>
            $"Cumulative probability of succeeding is {probability}";
    }
}
