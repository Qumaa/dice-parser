using System;
using System.Collections.Generic;

namespace DiceRoll.Input
{
    public interface IToken
    {
        bool Matches(ReadOnlySpan<char> token);

        IEnumerable<string> EnumerateRawTokens();
    }
}
