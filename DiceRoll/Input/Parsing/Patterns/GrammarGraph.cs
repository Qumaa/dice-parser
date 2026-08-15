using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class GrammarGraph
    {
        private readonly GrammarChainProvider _chainProvider;
        
        public GrammarGraph(GrammarChainProvider chainProvider)
        {
            _chainProvider = chainProvider;
        }

        public void Parse(string input)
        {
            
        }

        public bool TryGetGrammarByTag(string tag, out IGrammar grammar)
        {
            throw new NotImplementedException();
        }
    }
}
