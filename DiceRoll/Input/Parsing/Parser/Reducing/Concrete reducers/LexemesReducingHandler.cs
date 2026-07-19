using System;

namespace DiceRoll.Input.Parsing
{
    public abstract class LexemesReducingHandler
    {
        public abstract Range Reduce(EquationParserState state, in Range range);
    }
}
