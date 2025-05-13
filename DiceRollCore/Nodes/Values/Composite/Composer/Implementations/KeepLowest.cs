namespace DiceRoll
{
    public sealed class KeepLowest : Composer
    {
        protected override INumeric Compose(INumeric[] source, ComposerContext context) =>
            IteratePairs(source, context, static (left, right) => Node.Operator.SelectLowest(left, right));
    }
}
