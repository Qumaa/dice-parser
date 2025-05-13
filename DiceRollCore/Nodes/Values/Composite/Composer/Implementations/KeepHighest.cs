namespace DiceRoll
{
    public sealed class KeepHighest : Composer
    {
        protected override INumeric Compose(INumeric[] source, ComposerContext context) =>
            IteratePairs(source, context, static (left, right) => Node.Operator.SelectHighest(left, right));
    }
}
