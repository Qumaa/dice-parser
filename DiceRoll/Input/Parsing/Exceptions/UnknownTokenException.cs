using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class UnknownTokenException : Exception
    {
        public UnknownTokenException(in Substring tokenMatch) : base(GetMessage(tokenMatch)) { }

        private static string GetMessage(in Substring tokenMatch) =>
            ParsingErrorMessages.UnknownToken(in tokenMatch);
    }
}
