using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class DiceToken : IToken
    {
        private readonly DieToken _die;
        
        public DiceToken(DieToken die)
        {
            ArgumentNullException.ThrowIfNull(die);
            
            _die = die;
        }

        public DiceToken(IToken delimiter) : this(new DieToken(delimiter)) { }

        public bool Matches(in Substring input, out Substring firstMatch)
        {
            firstMatch = input.Empty();

            if (!_die.Matches(in input, out Substring firstDie))
                return false;
            
            // ensure that there is no space to the left of the delimiter
            int lookupPos = firstDie.Start - 1;

            if (lookupPos < 0)
                return false; // input starts with delimiter

            if (char.IsWhiteSpace(input.Source[lookupPos]))
                return false; // forbid space

            firstMatch = firstDie;
            return true;
        }
    }
}
