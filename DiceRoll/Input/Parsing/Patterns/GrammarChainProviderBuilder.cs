using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    public class GrammarChainProviderBuilder
    {
        private readonly List<Tagged<IGrammar[]>> _chains = new();

        public void Add(string tag, IGrammar grammar)
        {
            if (grammar is IChainableGrammar chainableGrammar)
            {
                BreakDownIntoChain(tag, chainableGrammar);
                return;
            }

            AddSingleAsChain(tag, grammar);
        }

        private void BreakDownIntoChain(string tag, IChainableGrammar chainableGrammar) =>
            _chains.Add(new Tagged<IGrammar[]>(tag, chainableGrammar.ToArray()));

        private void AddSingleAsChain(string tag, IGrammar grammar) =>
            _chains.Add(new Tagged<IGrammar[]>(tag, new[] { grammar }));

        public GrammarChainProvider Build() =>
            new(_chains.ToArray());
    }
}
