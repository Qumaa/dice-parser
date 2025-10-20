using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    public sealed class CompositeToken : IToken
    {
        private readonly IToken[] _tokens;

        public CompositeToken(IEnumerable<IToken> tokens) : this(Syntax.ToArray(tokens)) { }
        
        public CompositeToken(IToken[] tokens)
        {
            _tokens = tokens;
        }

        public bool Matches(in Substring input, out Substring matchSubstring)
        {
            matchSubstring = Substring.Empty(in input);
            
            foreach (IToken token in _tokens)
            {
                if (!token.Matches(in input, out Substring newMatch))
                    continue;

                if (matchSubstring.IsEmpty || newMatch.Start < matchSubstring.Start)
                    matchSubstring = newMatch;
            }

            return !matchSubstring.IsEmpty;
        }
    }

    public static class CompositeTokenExtensions
    {
        public static IToken ToCompositeToken(this IEnumerable<IToken> tokens) =>
            ToCompositeToken(Syntax.ToArray(tokens));

        public static IToken ToCompositeToken(this IToken[] tokens) =>
            tokens.Length is 1 ? tokens[0] : new CompositeToken(tokens);
    }
}
