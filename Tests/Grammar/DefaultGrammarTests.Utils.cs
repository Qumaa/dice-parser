namespace Tests.Syntax.Default
{
    // todo bad syntax assertions e.g. "1 +" throws not enough operands
    // todo bad type assertions e.g. "true" as numeric throws
    public partial class DefaultSyntaxTests
    {
        private static readonly IEquationParser _parser = CreateDefaultGrammarParser();

        private static IEquationParser CreateDefaultGrammarParser()
        {
            EquationParserState state = new();
            LexingPipeline lexingPipeline = TokensTable.Default.ToDefaultPipeline(state.Lexemes);
            LexemesReducingPipeline reducingPipeline = LexemesReducingPipeline.CreateDefault(OperandCastingTable.Default);
            
            return new EquationParser(state, lexingPipeline, reducingPipeline);
        }

        public static NodeTree Parse(string input)
        {
            _parser.AccumulateInput(input);

            return _parser.ParseAccumulatedInput(UnknownLexemeSolver.Inert);
        }

        public static T Parse<T>(string input) where T : INode =>
            (T) Parse(input).Root.Node;
    }
}
