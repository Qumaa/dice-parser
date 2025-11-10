using System.Collections.Generic;

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

        public bool Matches(in Substring input, out Substring firstMatch)
        {
            firstMatch = input.Empty();
            
            foreach (IToken token in _tokens)
            {
                if (!token.Matches(in input, out Substring newMatch))
                    continue;

                if (firstMatch.IsEmpty || newMatch.Start < firstMatch.Start)
                    firstMatch = newMatch;
            }

            return !firstMatch.IsEmpty;
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
