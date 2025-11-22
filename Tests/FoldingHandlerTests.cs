namespace Tests
{
    [TestClass]
    public class FoldingHandlerTests
    {
        private static readonly OperatorFoldingHandler _handler = new();


        [TestMethod]
        public void SimpleTest()
        {
            LexemesList list = Lex("2 - - 2");

            _handler.FoldOperators(list, Range.All);
        }

        private static LexemesList Lex(string input)
        {
            EquationParserState state = new(OperandCastingTable.Default);
            EquationReader reader = new(state, TokensTable.Default.ToDefaultPipeline(state));
            
            reader.Read(input);

            return state.Lexemes;
        }
    }
}
