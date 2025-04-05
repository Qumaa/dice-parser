using System;

namespace DiceRoll.Input
{
    public sealed class UnknownTokenException : Exception
    {
        private const string _MESSAGE_FIRST_LINE = "Couldn't parse the following unknown token: \"{0}\".";

        public UnknownTokenException(in MatchInfo tokenMatch) : base(BuildMessage(tokenMatch))
        {
        }

        public UnknownTokenException(in MatchInfo tokenMatch, Exception innerException) : base(BuildMessage(tokenMatch),
            innerException)
        {
        }

        private static string BuildMessage(in MatchInfo tokenMatch) =>
            string.Format(_MESSAGE_FIRST_LINE, tokenMatch.SliceMatch().ToString());
    }
}
