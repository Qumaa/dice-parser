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

            Range matchRange = input.SourceRangeToRelativeRange(delimiter.AsRange());
            
            // y
            Substring postDelimiter = input[input.SourceIndexToRelativeIndex(delimiter.End)..];
            
            if (!number.MatchesStart(postDelimiter, out Substring diceFaces))
                goto matchFailed;

            matchRange = matchRange.And(input.SourceRangeToRelativeRange(diceFaces.AsRange()));

            // x
            Substring preDelimiter = input[..input.SourceIndexToRelativeIndex(delimiter.Start)];
            
            if (number.MatchesEnd(in preDelimiter, out Substring diceNumber))
            {
                matchRange = matchRange.And(input.SourceRangeToRelativeRange(diceNumber.AsRange()));
                
                // c
                postDelimiter = postDelimiter.MoveStart(diceFaces.Length);
            
                if (_composition.MatchesStart(in postDelimiter, out Substring composition))
                    matchRange = matchRange.And(input.SourceRangeToRelativeRange(composition.AsRange()));
            }

            //
            matchSubstring = input[matchRange];
            return true;
            
            matchFailed:
            matchSubstring = default;
            return false;
        }
    }
}
