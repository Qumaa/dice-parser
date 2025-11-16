using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    public sealed class TokenGroupChain
    {
        private readonly TokenGroup[] _groups;

        public TokenGroupChain(IEnumerable<TokenGroup> groups)
        {
            ArgumentNullException.ThrowIfNull(groups);
            
            _groups = groups.OrderByDescending(x => x.Precedence).ToArray();
        }

        public bool TryExecuteAll(in Substring substring, out Substring match)
        {
            Substring earliestMatch = substring.Empty();
            
            foreach (TokenGroup group in _groups)
                if (group.TryExecute(in substring, out Substring newMatch))
                {
                    match = newMatch;
                    return true;
                }
                else
                    TokenGroupUtils.UpdateEarliestMatch(ref earliestMatch, in newMatch);

            match = earliestMatch.IsEmpty ? substring : substring.SetEnd(earliestMatch.Start).TrimEnd();
            return false;
        }
    }
}
