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

        public bool Matches(in Substring input, out Substring match)
        {
            foreach (IToken token in _tokens)
                if (token.Matches(in input, out match))
                    return true;

            match = default;
            return false;
        }
    }

    public static class CompositeTokenExtensions
    {
        public static IToken ToCompositeToken(this IEnumerable<IToken> tokens) =>
            ToCompositeToken(tokens as IToken[] ?? tokens.ToArray());

        public static IToken ToCompositeToken(this IToken[] tokens) =>
            tokens.Length is 1 ? tokens[0] : new CompositeToken(tokens);
    }
}
