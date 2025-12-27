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

        public override void Execute(LexemesList lexemes)
        {
            for (int i = 0; i < lexemes.Count; i++)
            {
                if (!lexemes.TryGetTyped(i, out Mapped<OpenParenthesis> _))
                    continue;
                
                DetermineRangeAndReduce(lexemes, i);
            }
        }

        private void DetermineRangeAndReduce(LexemesList lexemes, int openParenthesisPosition)
        {
            int position = openParenthesisPosition;

            do
            {
                position++;
                
                if (position >= lexemes.Count)
                    throw new Exception(); // todo

                if (lexemes.TryGetTyped(position, out Mapped<OpenParenthesis> _))
                    DetermineRangeAndReduce(lexemes, position);
            } while (!lexemes.TryGetTyped(position, out Mapped<CloseParenthesis> _));

            lexemes.Remove(position);
            lexemes.Remove(openParenthesisPosition);
            
            Range withinParenthesisRange = openParenthesisPosition..(position - 1);

            Range reducedRange = _operatorHandler.Reduce(lexemes, in withinParenthesisRange);
            _nodePoolHandler.Reduce(lexemes, in reducedRange);
        }
    }
}
