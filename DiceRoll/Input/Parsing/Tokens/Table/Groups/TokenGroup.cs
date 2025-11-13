namespace DiceRoll.Input.Parsing
{
    // todo default inheritors are internal
    // todo inline all inheritors delegated calls
    public abstract class TokenGroup
    {
        public readonly int Precedence;
        
        protected TokenGroup(int precedence)
        {
            Precedence = precedence;
        }

        public abstract bool TryMatch(in Substring substring, out Substring match);
    }
}
