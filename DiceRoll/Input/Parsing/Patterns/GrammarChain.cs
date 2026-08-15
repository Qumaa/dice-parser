namespace DiceRoll.Input.Parsing
{
    public class GrammarChain : IGrammar
    {
        private readonly IGrammar[] _chain;
        private int _state;

        public GrammarChain(IGrammar[] chain)
        {
            _chain = chain;
        }

        public bool TryAdvance(ParseContext context)
        {
            if (_state >= _chain.Length)
                return false;

            IGrammar current = _chain[_state];

            GrammarProbe probe = current.ProbeContext(context);
            
            if (!probe.IsSuccessful)
                return false;

            _state++;
            return true;
        }

        GrammarProbe IGrammar.ProbeContext(ParseContext context) =>
            _chain.ProbeAllSequentially(context);
    }
}