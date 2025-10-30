using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public sealed class NodePoolOperandBuilder
    {
        private readonly List<IToken> _openScope = new();
        private readonly List<IToken> _closeScope = new();
        private readonly List<IToken> _separators = new();

        public NodePoolOperandBuilder OpenScope(IToken token)
        {
            _openScope.Add(token);
            return this;
        }

        public NodePoolOperandBuilder CloseScope(IToken token)
        {
            _closeScope.Add(token);
            return this;
        }

        public NodePoolOperandBuilder Separator(IToken token)
        {
            _separators.Add(token);
            return this;
        }

        public OperandDefinition Build()
        {
            IToken openScope = _openScope.ToCompositeToken();
            IToken closeScope = _closeScope.ToCompositeToken();
            IToken separator = _separators.ToCompositeToken();

            NodePoolParsingHelper helper = new(openScope, closeScope, separator);

            NodePoolToken token = new(helper);
            NodePoolParser parser = new(helper);
            
            return OperandDefinition.OfType<INode>(token, parser);
        }
    }
}
