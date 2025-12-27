namespace Tests
{
    [TestClass]
    public class FoldingHandlerTests
    {
        private static readonly OperatorReducingHandler _operatorHandler = new(OperandCastingTable.Default);
        private static readonly NodePoolReducingHandler _nodePoolHandler = new();

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

            _operatorHandler.Reduce(list, Range.All);
        }

        [TestMethod]
        public void LimitedRangeTest()
        {
            LexemesList list = Lex("2 - 1 + 1");
            Range range = 1..;

            _operatorHandler.Reduce(list, in range);
        }


        [TestMethod]
        public void NodePoolSimpleTest()
        {
            LexemesList list = Lex("2 1 4");

            _nodePoolHandler.Reduce(list, Range.All);
        }
        
        [TestMethod]
        public void NodePoolLimitedRangeTest()
        {
            LexemesList list = Lex("2 1 4");

            _nodePoolHandler.Reduce(list, ..^1);
        }
    }
}
