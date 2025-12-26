namespace Tests
{
    [TestClass]
    public class FoldingHandlerTests
    {
        private static readonly OperatorFoldingHandler _handler = new(OperandCastingTable.Default);

        private static LexemesList Lex(string input)
        {
            EquationParserState state = new();
            EquationReader reader = new(state.Mapper, state.Lexemes, TokensTable.Default.ToDefaultPipeline(state.Lexemes));
            
            reader.Read(input);

            return state.Lexemes;
        }

        // todo proper tests
        
        [TestMethod]
        public void SimpleTest()
        {
            LexemesList list = Lex("2 - - 2");

            _handler.FoldOperators(list, Range.All);
        }

        [TestMethod]
        public void LimitedRangeTest()
        {
            LexemesList list = Lex("2 - 1 + 1");
            Range range = 1..;

            _handler.FoldOperators(list, in range);
        }
    }
}
