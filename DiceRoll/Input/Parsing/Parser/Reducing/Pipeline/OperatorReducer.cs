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

        public override void Execute(EquationParserState state, UnknownLexemeSolver solver) =>
            _handler.Reduce(state, Range.All);
    }
}
