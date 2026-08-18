using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public sealed class GrammarCollection
    {
        private readonly Dictionary<string, IGrammar> _grammars;
        private readonly Dictionary<RelationPair, Relation> _relations;

        public GrammarCollection(
            Dictionary<string, IGrammar> grammars,
            Dictionary<RelationPair, Relation> relations
            )
        {
            _grammars = grammars;
            _relations = relations;
        }

        public bool TryGetGrammarByTag(string tag, out IGrammar grammar) =>
            _grammars.TryGetValue(tag, out grammar);
    }
}
