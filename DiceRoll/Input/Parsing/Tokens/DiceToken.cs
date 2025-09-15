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

        public bool Matches(in Substring input, out Substring match)
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

            Range matchRange = delimiter.AsRelativeRange(in input);
            
            // y
            Substring postDelimiter = input[delimiter.RelativeEnd(in input)..];
            
            if (!number.MatchesStart(postDelimiter, out Substring diceFaces))
                goto matchFailed;

            matchRange = matchRange.And(diceFaces.AsRelativeRange(in input));

            // c
            postDelimiter = postDelimiter.MoveStart(diceFaces.Length).TrimStart();
            
            if (_composition.MatchesStart(in postDelimiter, out Substring composition))
                matchRange = matchRange.And(composition.AsRelativeRange(in input));
            
            // x
            Substring preDelimiter = input[..delimiter.RelativeStart(in input)];
            
            if (number.MatchesEnd(in preDelimiter, out Substring diceNumber))
                matchRange = matchRange.And(diceNumber.AsRelativeRange(in input));

            //
            (int start, int length) = matchRange.GetOffsetAndLength(input.Length);
            
            match = new Substring(in input, start, length);
            return true;
            
            matchFailed:
            match = default;
            return false;
        }
    }
}
