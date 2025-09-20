namespace DiceRoll.Input.Parsing
{
    public interface IToken
    {
        bool Matches(in Substring input, out Substring matchSubstring);
    }

    public static class TokenExtensions
    {
        public static bool Matches(this IToken token, in Substring input) =>
            token.Matches(input, out _);

        public static bool MatchesStart(this IToken token, in Substring input, out Substring matchSubstring) =>
            token.Matches(in input, out matchSubstring) && matchSubstring.Start == input.Start;
        
        public static bool MatchesEnd(this IToken token, in Substring input, out Substring matchSubstring) =>
            token.Matches(in input, out matchSubstring) && matchSubstring.End == input.End;
        
        public static bool Matches(this IToken token, string input) =>
            token.Matches(Substring.All(input), out _);
    }
}
