using System;

namespace DiceRoll.Input
{
    public sealed class UnknownTokenException : Exception
    {
        private const string _MESSAGE_FIRST_LINE = "Couldn't parse the following unknown token: \"{0}\".";

        public UnknownTokenException(in Substring tokenMatch) : base(BuildMessage(tokenMatch))
        {
        }

        public UnknownTokenException(in Substring tokenMatch, Exception innerException) : base(BuildMessage(tokenMatch),
            innerException)
        {
        }

        private static string BuildMessage(in Substring tokenMatch) =>
            string.Format(_MESSAGE_FIRST_LINE, tokenMatch.AsSpan().ToString());
    }
}
