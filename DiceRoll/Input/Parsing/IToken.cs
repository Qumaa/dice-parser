namespace DiceRoll.Input
{
    public interface IToken
    {
        bool Matches(in Substring input, out Substring substring);
    }

    public static class TokenExtensions
    {
        public static bool Matches(this IToken token, in Substring input) =>
            token.Matches(input, out _);
        public static bool Matches(this IToken token, string input) =>
            token.Matches(Substring.All(input), out _);
    }
}
