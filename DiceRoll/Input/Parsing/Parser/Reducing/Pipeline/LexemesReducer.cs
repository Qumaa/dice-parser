namespace DiceRoll.Input.Parsing
{
    public abstract class LexemesReducer
    {
        public abstract void Execute(EquationParserState state, UnknownLexemeSolver solver);
    }
}
