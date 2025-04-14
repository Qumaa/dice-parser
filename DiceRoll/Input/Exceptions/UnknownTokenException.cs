using System;

namespace DiceRoll.Input
{
    public sealed class UnknownTokenException : Exception
    {
        public UnknownTokenException(in Substring tokenMatch) : base(GetMessage(tokenMatch)) { }

        private static string GetMessage(in Substring tokenMatch) =>
            ParsingErrorMessages.UnknownToken(in tokenMatch);
    }
}
