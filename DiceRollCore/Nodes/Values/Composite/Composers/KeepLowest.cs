namespace DiceRoll
{
    public sealed class KeepLowest : Composer
    {
        protected override INumeric Compose(INumeric[] source) =>
            IteratePairs(source, static (left, right) => Node.Operator.SelectLowest(left, right));
    }
}
