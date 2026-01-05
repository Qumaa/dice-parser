namespace Tests.Grammar.Default
{
    // todo bad syntax assertions e.g. "1 +" throws not enough operands
    // todo bad type assertions e.g. "true" as numeric throws
    public partial class DefaultGrammarTests
    {
        private static readonly IEquationParser _parser = CreateDefaultGrammarParser();

        private static IEquationParser CreateDefaultGrammarParser()
        {
            EquationParserState state = new();
            LexingPipeline lexingPipeline = TokensTable.Default.ToDefaultPipeline(state.Lexemes);
            LexemesReducingPipeline reducingPipeline = LexemesReducingPipeline.CreateDefault(OperandCastingTable.Default);
            
            return new EquationParser(state, lexingPipeline, reducingPipeline);
        }
        
        private static T Parse<T>(string input) where T : INode
        {
            _parser.AccumulateInput(input);

            return (T) _parser.ParseAccumulatedInput(UnknownLexemeSolver.Inert).Root.Value.Node;
        }

        private static T GetTemplate<T>() where T : Template, new() =>
            new();

        private abstract class Template
        {
            public readonly string[] SampleStrings;
            
            protected Template(string sampleString)
            {
                SampleStrings = [sampleString];
            }

            protected Template(string formatString, params string[] options)
            {
                SampleStrings = options.Select(x => string.Format(formatString, x)).ToArray();
            }

            public bool MeetsExpectedPattern(INode[] nodes) =>
                nodes.All(MeetsExpectedPattern);

            protected abstract bool MeetsExpectedPattern(INode node);
        }

        private static class AssertParsingResultOf<TTemplate> where TTemplate : Template, new()
        {
            private static readonly TTemplate _template;
            private static readonly INode[] _sampleParseResult;

            static AssertParsingResultOf()
            {
                _template = GetTemplate<TTemplate>();
                _sampleParseResult = _template.SampleStrings.Select(Parse<INode>).ToArray();
            }
            
            public static void FailsToMeetExpectedPatternOf<TOther>() where TOther : Template, new()
            {
                TOther otherTemplate = AssertParsingResultOf<TOther>._template;
                
                if (otherTemplate.MeetsExpectedPattern(_sampleParseResult))
                    Assert.Fail($"Parsing a sample of {typeof(TTemplate).Name} produced a result that also meets the expected pattern of {typeof(TOther).Name}, which it must not.");
            }

            public static void MeetsExpectedPattern()
            {
                if (!_template.MeetsExpectedPattern(_sampleParseResult))
                    Assert.Fail($"Parsing a sample of {typeof(TTemplate).Name} produced a result that doesn't meet the expected pattern.");
            }
        }
    }
}
