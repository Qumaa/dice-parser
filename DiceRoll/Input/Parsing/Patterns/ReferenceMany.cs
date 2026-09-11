namespace DiceRoll.Input.Parsing
{
    public sealed class ReferenceMany : IGrammar
    {
        private readonly string[] _tags;
        
        public ReferenceMany(string[] tags)
        {
            _tags = tags;
        }

        public GrammarProbe ProbeContext(ParseContext context)
        {
            foreach (string tag in _tags)
                if (context.TryGetGrammarByTag(tag, out IGrammar grammar))
                    return grammar.ProbeContext(context);

            return GrammarProbe.Failed;

        }
    }
}
