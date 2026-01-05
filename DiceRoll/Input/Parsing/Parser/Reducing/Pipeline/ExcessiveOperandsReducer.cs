using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class ExcessiveOperandsReducer : LexemesReducer
    {
        private readonly SequenceReducingHandler _handler;
        
        public ExcessiveOperandsReducer(SequenceReducingHandler handler)
        {
            ArgumentNullException.ThrowIfNull(handler);
            
            _handler = handler;
        }

        public override void Execute(LexemesList lexemes, Cursor cursor) =>
            _handler.Reduce(lexemes, Range.All, cursor);
    }
}
