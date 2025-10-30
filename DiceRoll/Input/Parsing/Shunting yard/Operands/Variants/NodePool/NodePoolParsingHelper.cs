using System;

namespace DiceRoll.Input.Parsing
{
    internal sealed class NodePoolParsingHelper
    {
        public readonly IToken OpenScope;
        public readonly IToken CloseScope;
        public readonly IToken Separator;
        
        public NodePoolParsingHelper(IToken openScope, IToken closeScope, IToken separator)
        {
            ArgumentNullException.ThrowIfNull(openScope);
            ArgumentNullException.ThrowIfNull(closeScope);
            ArgumentNullException.ThrowIfNull(separator);
            
            OpenScope = openScope;
            CloseScope = closeScope;
            Separator = separator;
        }

        public bool TryGetMatchingCloseScope(in Substring afterScopeOpened, out Substring closeScope)
        {
            int balance = 1;
            Substring search = afterScopeOpened;
            
            while (CloseScope.Matches(in search, out closeScope))
            {
                balance--;

                Substring untilCloseScope = search.SetEnd(closeScope.Start);

                foreach (Substring _ in OpenScope.EnumerateMatches(untilCloseScope))
                    balance++;

                if (balance <= 0)
                    return true;

                search = search.SetStart(closeScope.End);
            }

            return false;
        }
        
        public bool TryGetNextMember(in Substring members, out Substring member, out bool isLast)
        {
            if (!Separator.Matches(in members, out Substring separator))
                goto last;
            
            Substring untilNextSeparator = members.SetEnd(separator.Start);
                
            while (OpenScope.Matches(untilNextSeparator, out Substring openScope))
            {
                if (!TryGetMatchingCloseScope(members.SetStart(openScope.End), out Substring closeScope))
                    goto fail;

                if (!Separator.Matches(members.SetStart(closeScope.End), out separator)) // has separator after close scope?
                    goto last;

                untilNextSeparator = members.Set(closeScope.End..separator.Start);
            }

            member = members.SetEnd(separator.Start);
            
            if (member.IsEmptyOrWhiteSpace())
                goto fail;
            
            isLast = false;
            return true;
            
            last:
            if (!members.IsEmptyOrWhiteSpace())
            {
                member = members;
                isLast = true;
                return true;
            }
            
            fail:
            member = members.Empty();
            isLast = false;
            return false;
        }
    }
}
