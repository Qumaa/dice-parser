using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class OperatorReducer : LexemesReducer
    {
        private readonly OperatorReducingHandler _handler;
        
        public OperatorReducer(OperatorReducingHandler handler)
        {
            ArgumentNullException.ThrowIfNull(handler);
            
            _handler = handler;
        }

        public override void Execute(LexemesList lexemes) =>
            _handler.Reduce(lexemes, Range.All);
    }
}
