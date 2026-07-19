namespace DiceRoll.Input.Parsing
{
    public sealed class EquationParserState
    {
        public readonly LexemesList Lexemes;
        public readonly InputMapper Mapper;
        public readonly Cursor Cursor;

        public EquationParserState()
        {
            Mapper = new InputMapper();
            Lexemes = new LexemesList(Mapper);
            Cursor = new Cursor();
        }

        public void Reset()
        {
            Lexemes.Clear();
            Mapper.Clear();
            Cursor.Clear();
        }
    }
}
