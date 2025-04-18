namespace DiceRoll
{
    internal enum AnalyzeOutputStyle
    {
        Full,
        OmitRolls,
        OmitFailure,
        OmitSuccess,
        OnlyRolls,
        OnlyFailure,
        OnlySuccess
    }

    internal static class AnalyzeOutputStyleExtensions
    {
        public static bool OmitsCumulativeSuccess(this AnalyzeOutputStyle style) =>
            style.OmitsSuccess() || style is AnalyzeOutputStyle.OnlyRolls;

        public static bool OmitsSuccess(this AnalyzeOutputStyle style) =>
            style is AnalyzeOutputStyle.OmitSuccess or AnalyzeOutputStyle.OnlyFailure;

        public static bool OmitsCumulativeFailure(this AnalyzeOutputStyle style) =>
            style.OmitsFailure() || style is AnalyzeOutputStyle.OnlyRolls;

        public static bool OmitsFailure(this AnalyzeOutputStyle style) =>
            style is AnalyzeOutputStyle.OmitFailure or AnalyzeOutputStyle.OnlySuccess;

        public static bool OmitsRolls(this AnalyzeOutputStyle style) =>
            style is AnalyzeOutputStyle.OmitRolls or AnalyzeOutputStyle.OnlyFailure or AnalyzeOutputStyle.OnlySuccess;
    }
}
