namespace DiceRoll.Input
{
    public interface IToken
    {
        bool Matches(in Substring input, out Substring match);
    }

    public static class TokenExtensions
    {
        public static bool Matches(this IToken token, in Substring input) =>
            token.Matches(input, out _);

        public static bool MatchesStart(this IToken token, in Substring input, out Substring match) =>
            token.Matches(in input, out match) && match.Start == input.Start;
        
        public static bool Matches(this IToken token, string input) =>
            token.Matches(Substring.All(input), out _);
    }
}
