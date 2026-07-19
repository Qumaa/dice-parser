namespace DiceRoll.Input.Parsing
{
    // todo make inheritors logic accessible otherwise (mostly to ease custom implementations of external token solver)
    // e.g. make api to process an operand, an operator, open/close parenthesis etc.
    public abstract class Lexer
    {
        /// <summary>
        /// Executes lexer-associated logic if the start of <paramref name="substring"/> is matched. 
        /// </summary>
        /// <param name="substring">A substring to scan the start of.</param>
        /// <param name="cursor"></param>
        /// <param name="match">Contains either the earliest match or an empty substring when did not match.</param>
        /// <returns>True if matched the start of <paramref name="substring"/>, false if matched elsewhere or did not match.</returns>
        public abstract bool TryExecute(in Substring substring, Cursor cursor, out Substring match);
    }
}
