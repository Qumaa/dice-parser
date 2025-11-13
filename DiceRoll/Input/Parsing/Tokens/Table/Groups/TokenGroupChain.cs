using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    public sealed class TokenGroupChain
    {
        private readonly TokenGroup[] _groups;

        public TokenGroupChain(IEnumerable<TokenGroup> groups)
        {
            _groups = groups.OrderByDescending(x => x.Precedence).ToArray();
        }

        public bool TryMatchAny(in Substring substring, out Substring firstMatch)
        {
            foreach (TokenGroup group in _groups)
                if (group.TryMatch(in substring, out firstMatch))
                    return true;

            firstMatch = substring.Empty();
            return false;
        }
    }
}
