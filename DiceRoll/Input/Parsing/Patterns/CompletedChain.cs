namespace DiceRoll.Input.Parsing
{
    public sealed class CompletedChain
    {
        public readonly string Tag;
        public readonly IGrammar[] Grammars;
        public readonly GrammarProbe[] Probes;
        
        public CompletedChain(string tag, IGrammar[] grammars, GrammarProbe[] probes)
        {
            Tag = tag;
            Grammars = grammars;
            Probes = probes;
        }
    }
}
