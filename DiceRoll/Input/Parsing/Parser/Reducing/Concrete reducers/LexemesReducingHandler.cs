using System;

namespace DiceRoll.Input.Parsing
{
    public abstract class LexemesReducingHandler
    {
        public abstract Range Reduce(LexemesList lexemes, in Range range, Cursor cursor);
    }
}
