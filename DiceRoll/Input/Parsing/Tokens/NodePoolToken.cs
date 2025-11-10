using System;

namespace DiceRoll.Input.Parsing
{
    internal sealed class NodePoolToken : IToken
    {
        private readonly NodePoolParsingHelper _helper;
        
        public NodePoolToken(NodePoolParsingHelper helper)
        {
            ArgumentNullException.ThrowIfNull(helper);
            
            _helper = helper;
        }

        public bool Matches(in Substring input, out Substring firstMatch)
        {
            if (!_helper.OpenScope.Matches(in input, out Substring openScope))
                goto matchFailed;

            if (!_helper.TryGetMatchingCloseScope(input.SetStart(openScope.End), out Substring closeScope))
                goto matchFailed;

            Substring members = input.SetRange(openScope.End..closeScope.Start);

            bool encounteredAtLeastOneSeparator = false;
            
            while (_helper.TryGetNextMember(in members, out Substring nextMember, out bool isLast))
            {
                if (!isLast)
                {
                    if (!_helper.Separator.Matches(members.SetStart(nextMember.End), out Substring separator))
                        goto matchFailed;
                    
                    members = members.SetStart(separator.End);
                    encounteredAtLeastOneSeparator = true;
                    continue;
                }
                
                if (!encounteredAtLeastOneSeparator)
                    goto matchFailed;

                goto matchSucceed;
            }

            matchFailed:
            firstMatch = input.Empty();
            return false;

            matchSucceed:
            firstMatch = input.SetRange(openScope.Start..closeScope.End);
            return true;
        }
    }
}
