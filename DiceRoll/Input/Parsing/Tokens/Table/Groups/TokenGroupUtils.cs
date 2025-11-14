namespace DiceRoll.Input.Parsing
{
    internal static class TokenGroupUtils
    {
        public static void UpdateEarliestMatch(ref Substring current, in Substring nextMatch)
        {
            if (current.IsEmpty || (!nextMatch.IsEmpty && nextMatch.Start < current.Start))
                current = nextMatch;
        }
    }
}
