using System;
using System.Collections.Generic;

namespace DiceRoll.Input
{
    public interface IToken
    {
        bool Matches(in ReadOnlySpan<char> token);

        IEnumerable<string> EnumerateRawTokens();
    }
}
