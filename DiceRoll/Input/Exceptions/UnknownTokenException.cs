using System;

namespace DiceRoll.Input
{
    public sealed class UnknownTokenException : Exception
    {
        private const string _MESSAGE = "Couldn't parse the following token: \"{0}\".";
        
        public UnknownTokenException(in ReadOnlySpan<char> token) : this(token.ToString()) { }
        public UnknownTokenException(string token) : base(FormatMessage(token)) { }
        public UnknownTokenException(in ReadOnlySpan<char> token, Exception innerException) : this(token.ToString(), innerException) { }
        public UnknownTokenException(string token, Exception innerException) : base(FormatMessage(token), innerException) { }

        private static string FormatMessage(string token) =>
            string.Format(_MESSAGE, token);
    }
}
