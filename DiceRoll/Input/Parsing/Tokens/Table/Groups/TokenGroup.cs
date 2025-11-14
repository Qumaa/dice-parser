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

        /// <summary>
        /// Executes group-associated logic if the start of <paramref name="substring"/> is matched. 
        /// </summary>
        /// <param name="substring">A substring to scan the start of.</param>
        /// <param name="match">Contains either the earliest match or an empty substring when did not match.</param>
        /// <returns>True if matched the start of <paramref name="substring"/>, false if matched elsewhere or did not match.</returns>
        public abstract bool TryMatchStart(in Substring substring, out Substring match);
    }
}
