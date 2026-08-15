namespace DiceRoll.Input.Parsing
{
    public sealed class Reference : IGrammar
    {
        private readonly string _tag;
        
        public Reference(string tag)
        {
            _tag = tag;
        }

        public GrammarProbe ProbeContext(ParseContext context)
        {
            if (!context.TryGetGrammarByTag(_tag, out IGrammar grammar))
                return GrammarProbe.Failed;

            return grammar.ProbeContext(context);
        }
    }
}
