namespace DiceRoll.Input.Parsing
{
    public abstract class LexemesReducer
    {
        public abstract void Execute(LexemesList lexemes, ReducerCursor cursor);
    }
}
