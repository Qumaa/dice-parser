namespace DiceRoll
{
    public sealed class KeepHighest : Composer
    {
        protected override INumeric Compose(INumeric[] source) =>
            IteratePairs(source, static (left, right) => Node.Operator.SelectHighest(left, right));
    }
}
