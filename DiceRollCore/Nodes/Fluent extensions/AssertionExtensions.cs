namespace DiceRoll.FluentExtensions
{
    public static class AssertionExtensions
    {
        public static IAssertion AsConstant(this bool value) =>
            Node.Value.Constant(value);
    }
}
