namespace DiceRoll
{
    public sealed class Summarize : Composer
    {
        protected override INumeric Compose(INumeric[] source, ComposerContext context) =>
            IteratePairs(source, context, static (left, right) => Node.Operator.Add(left, right));
    }
}
