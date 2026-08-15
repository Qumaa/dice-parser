namespace DiceRoll.Input.Parsing
{
    public sealed class Literal : IGrammar
    {
        private readonly IToken _token;
        
        public Literal(IToken token)
        {
            _token = token;
        }

        public GrammarProbe ProbeContext(ParseContext context)
        {
            if (!_token.MatchesStart(context.GetUnrecognized(), out Substring match))
                return GrammarProbe.Failed;

            return new GrammarProbe(in match);
        }
    }
}
