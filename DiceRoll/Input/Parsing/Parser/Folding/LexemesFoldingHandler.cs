using System;

namespace DiceRoll.Input.Parsing
{
    public abstract class LexemesFoldingHandler
    {
        public abstract Range Fold(LexemesList lexemes, in Range range);
    }
}
