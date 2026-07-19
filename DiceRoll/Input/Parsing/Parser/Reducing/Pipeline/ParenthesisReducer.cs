using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class ParenthesisReducer : LexemesReducer
    {
        private readonly OperatorReducingHandler _operatorHandler;
        private readonly SequenceReducingHandler _sequenceHandler;
        
        public ParenthesisReducer(OperatorReducingHandler operatorHandler, SequenceReducingHandler sequenceHandler)
        {
            ArgumentNullException.ThrowIfNull(operatorHandler);
            ArgumentNullException.ThrowIfNull(sequenceHandler);
            
            _operatorHandler = operatorHandler;
            _sequenceHandler = sequenceHandler;
        }

        public override void Execute(EquationParserState state, UnknownLexemeSolver solver)
        {
            for (int i = 0; i < state.Lexemes.Count; i++)
            {
                if (state.Lexemes.TryGetTyped(i, out Mapped<CloseParenthesis> close))
                {
                    state.Cursor.MoveTo(close.Range);
                    throw new Exception("Unmatched closing parenthesis.");
                }

                if (!state.Lexemes.TryGetTyped(i, out Mapped<OpenParenthesis> openParenthesis))
                    continue;

                DetermineRangeAndReduce(state, i, in openParenthesis.Range);
            }
        }

        private void DetermineRangeAndReduce(EquationParserState state, int openParenthesisPosition, in Range openParenthesisRange)
        {
            int position = openParenthesisPosition;
            state.Cursor.MoveTo(in openParenthesisRange);

            do
            {
                position++;
                
                if (position >= state.Lexemes.Count)
                    throw new Exception("Unmatched opening parenthesis.");

                if (state.Lexemes.TryGetTyped(position, out Mapped<OpenParenthesis> open))
                    DetermineRangeAndReduce(state, position, in open.Range);
            } while (!state.Lexemes.TryGetTyped(position, out Mapped<CloseParenthesis> _));
            
            state.Cursor.MoveToPrevious();

            state.Lexemes.Remove(position);
            state.Lexemes.Remove(openParenthesisPosition);
            
            Range withinParenthesisRange = openParenthesisPosition..(position - 1);
            
            Range reducedRange = _operatorHandler.Reduce(state, in withinParenthesisRange);
            
            _sequenceHandler.Reduce(state, in reducedRange);
        }
    }
}
