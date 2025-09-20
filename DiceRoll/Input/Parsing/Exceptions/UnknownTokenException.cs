using System;

namespace DiceRoll.Input.Parsing
{
    internal sealed class UnknownTokenException : Exception
    {
        public UnknownTokenException(in Substring tokenMatch) : base(
            $"This token is not recognized (\"{tokenMatch}\")."
            ) { }
    }
}
