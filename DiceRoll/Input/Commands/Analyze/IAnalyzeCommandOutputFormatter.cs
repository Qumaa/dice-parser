namespace DiceRoll
{
    public interface IAnalyzeCommandOutputFormatter
    {
        string Rolling(Outcome outcome, Probability probability);
        string Asserting(in Logical logical);
        string CumulativeFailing(Probability probability);
        string CumulativeSucceeding(Probability probability);
    }

    public static class AnalyzeCommandOutputFormatterExtensions
    {
        public static string Asserting(this IAnalyzeCommandOutputFormatter formatter, bool value,
            Probability probability) =>
            formatter.Asserting(new Logical(value, probability));

        public static string AssertingTrue(this IAnalyzeCommandOutputFormatter formatter, Probability probability) =>
            formatter.Asserting(true, probability);
        
        public static string AssertingFalse(this IAnalyzeCommandOutputFormatter formatter, Probability probability) =>
            formatter.Asserting(false, probability);
    }
}
