using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class DieToken : IToken
    {
        private readonly IToken _delimiter;
        
        public DieToken(IToken delimiter)
        {
            ArgumentNullException.ThrowIfNull(delimiter);
            
            _delimiter = delimiter;
        }

        public bool Matches(in Substring input, out Substring firstMatch)
        {
            firstMatch = input.Empty();

            if (!_delimiter.Matches(in input, out Substring firstDelimiter))
                return false;
            
            // ensure that there is no space to the right of the delimiter
            int lookupPos = firstDelimiter.End - input.Start;

            if (lookupPos >= input.Length)
                return false; // input ends with delimiter

            if (char.IsWhiteSpace(input[lookupPos]))
                return false; // forbid space

            firstMatch = firstDelimiter;
            return true;
        }
    }
}
