namespace DiceRoll.Input.Parsing
{
    public static class MatchUtils
    {
        public static bool ShouldUpdateMatch(in Substring firstMatch, int newMatchStart, int newMatchLength)
        {
            if (newMatchLength is 0)
                return false;
            
            if (firstMatch.IsEmpty)
                return true;
            
            if (newMatchStart > firstMatch.Start) // match is encountered later than the current one
                return false;
            
            if (newMatchStart == firstMatch.Start && newMatchLength <= firstMatch.Length) // match starts at the same position but is shorter 
                return false;

            return true;
        }
    }
}
