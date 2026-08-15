using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class GrammarGraphBuilder
    {
        private readonly GrammarChainProviderBuilder _chainProviderBuilder = new();
        private readonly GrammarRelationsBuilder _relationsBuilder = new();

        public GrammarGraphBuilder Add(string tag, IGrammar grammar)
        {
            throw new NotImplementedException();
        }

        public GrammarGraphBuilder Relate(string tag, string tagTo, Relation relation)
        {
            _relationsBuilder.Relate(tag, tagTo, relation);
            return this;
        }
    }
}