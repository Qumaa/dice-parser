namespace DiceRoll.Input.Parsing
{
    public sealed class TokenDrivenGrammar : IGrammar
    {
        private readonly IToken _token;
        
        public TokenDrivenGrammar(IToken token)
        {
            _token = token;
        }

        public GrammarProbe ProbeContext(ParseContext context)
        {
            if (!_token.MatchesStart(context.GetUnrecognized().TrimStart(), out Substring match))
                return GrammarProbe.Failed;

            return new GrammarProbe(in match);
        }
    }
}
