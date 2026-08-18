using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    public class GrammarChainProviderBuilder
    {
        private readonly List<IGrammar[]> _chains = new();

        public void Add(IGrammar grammar)
        {
            if (grammar is IChainableGrammar chainableGrammar)
            {
                BreakDownIntoChain(chainableGrammar);
                return;
            }

            AddSingleAsChain(grammar);
        }

        private void BreakDownIntoChain(IChainableGrammar chainableGrammar) =>
            _chains.Add(chainableGrammar.ToArray());

        private void AddSingleAsChain(IGrammar grammar) =>
            _chains.Add(new[] { grammar });

        public GrammarChainProvider Build() =>
            new(_chains.ToArray());
    }
}
