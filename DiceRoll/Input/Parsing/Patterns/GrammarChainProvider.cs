using System;

namespace DiceRoll.Input.Parsing
{
    public class GrammarChainProvider
    {
        private readonly IGrammar[][] _chains;

        public GrammarChainProvider(IGrammar[][] chains)
        {
            _chains = chains;
        }
        // provides any that start with the current token

        public GrammarChain[] GetChainsThatStartWith(ParseContext context)
        {
            throw new NotImplementedException();
        }
    }
}