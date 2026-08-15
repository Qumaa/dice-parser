using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class ParseContext
    {
        private readonly GrammarGraph _graph;
        private readonly Substring _source;
        
        public ParseContext(Substring source, GrammarGraph graph)
        {
            _source = source;
            _graph = graph;
        }

        public Substring GetUnrecognized()
        {
            throw new NotImplementedException();
        }

        public bool TryGetGrammarByTag(string tag, out IGrammar grammar) =>
            _graph.TryGetGrammarByTag(tag, out grammar);
    }
}