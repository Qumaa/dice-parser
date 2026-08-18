using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public sealed class GrammarCollectionBuilder
    {
        private readonly Dictionary<string, IGrammar> _grammars = new();
        private readonly Dictionary<RelationPair, Relation> _relations = new();

        public bool TryAdd(string tag, IGrammar grammar) =>
            _grammars.TryAdd(tag, grammar);
        
        public bool TryRelate(string tag, string tagTo, Relation relation) =>
            _relations.TryAdd(new RelationPair(tag, tagTo), relation);

        public GrammarCollection Build() =>
            new(_grammars, _relations);
    }
}
