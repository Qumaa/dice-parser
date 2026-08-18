namespace Tests.Syntax.Default
{
    public static class TemplateCache<T> where T : VerboseTemplate
    {
        public static T? Instance { get; private set; }
        public static NodeTree[]? SampleParseResult { get; private set; }
        public static bool IsEmpty => Instance is null;

        public static void CacheInstanceAndParseSamples(T instance)
        {
            CacheInstance(instance);
            CacheParseResult(instance.SampleStrings.Select(DefaultSyntaxTests.Parse).ToArray());
        }

        public static void CacheInstance(T instance) =>
            Instance = instance;

        public static void CacheParseResult(NodeTree[] parseResult) =>
            SampleParseResult = parseResult;

        public static void AssertFailsToMeetExpectedPatternOf<TOther>(TOther other) where TOther : VerboseTemplate
        {
            if (IsEmpty)
                return;
                
            if (other.MeetsExpectedPattern(SampleParseResult!))
                Assert.Fail($"Parsing a sample of {typeof(T).Name} produced a result that also meets the expected pattern of {typeof(TOther).Name}, which it must not.");
        }

        public static void AssertMeetsExpectedPattern()
        {
            if (IsEmpty)
                return;
                
            if (!Instance!.MeetsExpectedPattern(SampleParseResult!))
                Assert.Fail($"Parsing a sample of {typeof(T).Name} produced a result that doesn't meet the expected pattern.");
        }
    }
}
