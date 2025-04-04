using System;
using System.Collections.Generic;

namespace DiceRoll.Input
{
    public interface IToken
    {
        bool Matches(ReadOnlySpan<char> token, out MatchInfo matchInfo);

        IEnumerable<string> EnumerateRawTokens();
    }

    public static class TokenExtensions
    {
        public static bool Matches(this IToken itoken, ReadOnlySpan<char> token) =>
            itoken.Matches(token, out _);
    }
}
