namespace DiceRoll.Input.Parsing
{
    public sealed class GrammarGraphBuilder
    {
        private readonly GrammarCollectionBuilder _collectionBuilder = new();
        private readonly GrammarChainProviderBuilder _chainProviderBuilder = new();

        public GrammarGraphBuilder Add(string tag, IGrammar grammar)
        {
            if (!_collectionBuilder.TryAdd(tag, grammar))
                return this;
            
            _chainProviderBuilder.Add(tag, grammar);
            return this;
        }

        public GrammarGraphBuilder Relate(string tag, string tagTo, Relation relation)
        {
            _collectionBuilder.TryRelate(tag, tagTo, relation);
            return this;
        }

        public GrammarGraph Build() =>
            new(_chainProviderBuilder.Build(), _collectionBuilder.Build());
    }
}
