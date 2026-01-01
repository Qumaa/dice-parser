using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class ParenthesisReducer : LexemesReducer
    {
        private readonly OperatorReducingHandler _operatorHandler;
        private readonly NodePoolReducingHandler _nodePoolHandler;
        
        public ParenthesisReducer(OperatorReducingHandler operatorHandler, NodePoolReducingHandler nodePoolHandler)
        {
            ArgumentNullException.ThrowIfNull(operatorHandler);
            ArgumentNullException.ThrowIfNull(nodePoolHandler);
            
            _operatorHandler = operatorHandler;
            _nodePoolHandler = nodePoolHandler;
        }

        public override void Execute(LexemesList lexemes, Cursor cursor)
        {
            for (int i = 0; i < lexemes.Count; i++)
            {
                if (!lexemes.TryGetTyped(i, out Mapped<OpenParenthesis> openParenthesis))
                    continue;

                DetermineRangeAndReduce(lexemes, i, cursor, in openParenthesis.Range);
            }
        }

        private void DetermineRangeAndReduce(LexemesList lexemes, int openParenthesisPosition, Cursor cursor,
            in Range openParenthesisRange)
        {
            int position = openParenthesisPosition;
            cursor.MoveTo(in openParenthesisRange);

            do
            {
                position++;
                
                if (position >= lexemes.Count)
                    throw new UnbalancedParenthesisException();

                if (lexemes.TryGetTyped(position, out Mapped<OpenParenthesis> open))
                    DetermineRangeAndReduce(lexemes, position, cursor, in open.Range);
            } while (!lexemes.TryGetTyped(position, out Mapped<CloseParenthesis> _));
            
            cursor.MoveToPrevious();

            lexemes.Remove(position);
            lexemes.Remove(openParenthesisPosition);
            
            Range withinParenthesisRange = openParenthesisPosition..(position - 1);
            
            Range reducedRange = _operatorHandler.Reduce(lexemes, in withinParenthesisRange, cursor);
            
            _nodePoolHandler.Reduce(lexemes, in reducedRange, cursor);
        }
    }
}
