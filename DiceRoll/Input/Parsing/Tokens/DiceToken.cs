using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class DiceToken : IToken
    {
        private readonly IToken _delimiter;
        private readonly IToken _composition;
        
        public DiceToken(IToken delimiterToken, IToken compositionToken)
        {
            _delimiter = delimiterToken;
            _composition = compositionToken;
        }

        public bool Matches(in Substring input, out Substring matchSubstring)
        {
            // xdyc
            // x = dice number (optional)
            // d = delimiter
            // y = dice faces
            // c = composition
            //
            // 2d12highest
            // xdyyccccccc
            
            NumericToken number = NumericToken.Shared;

            // d
            if (!_delimiter.Matches(in input, out Substring delimiter))
                goto matchFailed;

            Range matchRange = delimiter.AsRange();
            
            // y
            Substring postDelimiter = input.SetStart(delimiter.End);
            
            if (!number.MatchesStart(postDelimiter, out Substring diceFaces))
                goto matchFailed;

            matchRange = matchRange.And(diceFaces.AsRange());

            // x
            Substring preDelimiter = input.SetEnd(delimiter.Start);
            
            if (number.MatchesEnd(in preDelimiter, out Substring diceNumber))
            {
                matchRange = matchRange.And(diceNumber.AsRange());
                
                // c
                postDelimiter = postDelimiter.MoveStart(diceFaces.Length);
            
                if (_composition.MatchesStart(in postDelimiter, out Substring composition))
                    matchRange = matchRange.And(composition.AsRange());
            }

            //
            matchSubstring = input.SetRange(matchRange);
            return true;
            
            matchFailed:
            matchSubstring = default;
            return false;
        }
    }
}
