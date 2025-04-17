namespace DiceRoll
{
    public interface IAnalyzeCommandOutputFormatter
    {
        string Rolling(Outcome outcome, Probability probability);
        string Asserting(in Logical logical);
        string Failing(Probability probability);
        string Succeeding(Probability probability);
    }
}
