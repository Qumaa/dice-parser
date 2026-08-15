namespace DiceRoll.Input.Parsing
{
    public sealed class Optional : IGrammar
    {
        private readonly IGrammar _optional;
        
        public Optional(IGrammar optional)
        {
            _optional = optional;
        }

        public GrammarProbe ProbeContext(ParseContext context)
        {
            GrammarProbe probe = _optional.ProbeContext(context);

            if (probe.IsSuccessful)
                return probe;

            Substring empty = context.GetUnrecognized().Empty();

            return new GrammarProbe(empty);
        }
    }
}