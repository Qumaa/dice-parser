namespace Tests.Grammar.Default
{
    public static class GrammarAssertExtensions
    {
        public static ParseResultAsserter<T> ParsingResultOf<T>(this Assert assert) where T : VerboseTemplate, new()
        {
            if (TemplateCache<T>.IsEmpty)
                TemplateCache<T>.CacheInstanceAndParseSamples(new T());

            return new ParseResultAsserter<T>();
        }

        public static PrecedenceAsserterBuilder<TL> PrecedenceOf<TL>(this Assert assert) where TL : OperatorContract, new()
        {
            if (TemplateCache<TL>.IsEmpty)
                TemplateCache<TL>.CacheInstance(new TL());
            
            return new PrecedenceAsserterBuilder<TL>();
        }
        
        public readonly struct ParseResultAsserter<T> where T : VerboseTemplate
        {
            public void MeetsExpectedPattern() =>
                TemplateCache<T>.AssertMeetsExpectedPattern();
        
            public void FailsToMeetExpectedPatternOf<TOther>() where TOther : VerboseTemplate, new()
            {
                if (TemplateCache<TOther>.IsEmpty)
                    TemplateCache<TOther>.CacheInstanceAndParseSamples(new TOther());
                
                TOther otherTemplate = TemplateCache<TOther>.Instance!;
                
                TemplateCache<T>.AssertFailsToMeetExpectedPatternOf(otherTemplate);
            }
        }

        public readonly struct PrecedenceAsserterBuilder<TL> where TL : OperatorContract, new()
        {
            public PrecedenceAsserter<TL, TR> Over<TR>() where TR : OperatorContract, new()
            {
                if (TemplateCache<TR>.IsEmpty)
                    TemplateCache<TR>.CacheInstance(new TR());
                
                return new PrecedenceAsserter<TL, TR>();
            }
        }
        
        public readonly struct PrecedenceAsserter<TL, TR> where TL : OperatorContract, new() where TR : OperatorContract, new()
        {
            public void IsHigher() =>
                Assert(true);

            public void IsLower() =>
                Assert(false);

            private static void Assert(bool favorLeft)
            {
                if (TemplateCache<OperatorPrecedenceTemplate<TL, TR>>.IsEmpty ||
                    TemplateCache<OperatorPrecedenceTemplate<TL, TR>>.Instance!.FavorLeft != favorLeft)
                    TemplateCache<OperatorPrecedenceTemplate<TL, TR>>.CacheInstanceAndParseSamples(new OperatorPrecedenceTemplate<TL, TR>(favorLeft));
                    
                TemplateCache<OperatorPrecedenceTemplate<TL, TR>>.AssertMeetsExpectedPattern();
            }
        }
    }
}
