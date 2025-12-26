namespace DiceRoll.Input.Parsing
{
    public sealed class EquationParserState
    {
        public readonly LexemesList Lexemes;
        public readonly InputMapper Mapper;

        public EquationParserState()
        {
            Mapper = new InputMapper();
            Lexemes = new LexemesList(Mapper);
        }

        public void Reset()
        {
            Lexemes.Clear();
            Mapper.Clear();
        }
    }
}
