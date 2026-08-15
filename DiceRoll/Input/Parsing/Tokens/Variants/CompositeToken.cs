using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    public sealed class CompositeToken : IToken
    {
        private readonly IToken[] _tokens;

        public CompositeToken(IEnumerable<IToken> tokens) : this(tokens.ToArray()) { }
        
        public CompositeToken(IToken[] tokens)
        {
            _tokens = tokens;
        }

        public bool Matches(in Substring input, out Substring firstMatch)
        {
            firstMatch = input.Empty();
            
            foreach (IToken token in _tokens)
            {
                if (!token.Matches(in input, out Substring newMatch))
                    continue;

                if (MatchUtils.ShouldUpdateMatch(in firstMatch, newMatch.Start, newMatch.Length))
                    firstMatch = newMatch;
            }

            return !firstMatch.IsEmpty;
        }
    }

    public static class CompositeTokenExtensions
    {
        public static IToken ToCompositeToken(this IEnumerable<IToken> tokens) =>
            ToCompositeToken(tokens.ToArray());

        public static IToken ToCompositeToken(this IToken[] tokens) =>
            tokens.Length is 1 ? tokens[0] : new CompositeToken(tokens);
    }
}
