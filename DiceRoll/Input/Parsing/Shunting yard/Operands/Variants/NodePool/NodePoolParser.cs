using System;
using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    internal sealed class NodePoolParser : RecursiveOperandParser
    {
        private readonly NodePoolParsingHelper _helper;
        
        public NodePoolParser(NodePoolParsingHelper helper)
        {
            ArgumentNullException.ThrowIfNull(helper);
            
            _helper = helper;
        }

        public override INode Parse(in Substring expression, FlatOperandParser parser)
        {
            Substring members = TrimScopeSymbols(expression);
            List<INode> nodes = new();

            while (_helper.TryGetNextMember(in members, out Substring nextMember, out bool isLast))
            {
                INode node = parser.Parse(nextMember.Trim());
                nodes.Add(node);
                
                if (isLast)
                    return new NodePool(nodes);

                if (!_helper.Separator.Matches(members.SetStart(nextMember.End), out Substring separator))
                    throw new FormatException(); // couldn't advance until next separator whereas there must be one
                
                members = members.SetStart(separator.End);
            }
            
            // couldn't get next member
            throw new FormatException();
        }

        private Substring TrimScopeSymbols(Substring expression)
        {
            if (_helper.OpenScope.MatchesStart(expression, out Substring openScope))
                expression = expression.SetStart(openScope.End);

            if (_helper.CloseScope.MatchesEnd(expression, out Substring closeScope))
                expression = expression.SetEnd(closeScope.Start);

            return expression.Trim();
        }
    }
}
