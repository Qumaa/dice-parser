namespace DiceRoll.Input.Parsing
{
    public static class LexerUtils
    {
        public static void UpdateEarliestMatch(ref Substring current, in Substring nextMatch)
        {
            if (nextMatch.IsEmpty)
                return;
            
            if (current.IsEmpty)
            {
                current = nextMatch;
                return;
            }

            if (nextMatch.Start >= current.Start)
                return;

            current = nextMatch;
        }
    }
}
