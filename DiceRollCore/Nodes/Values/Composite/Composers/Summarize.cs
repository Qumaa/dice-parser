namespace DiceRoll
{
    public sealed class Summarize : Composer
    {
        protected override IComposite Compose(INumeric[] source) =>
            IteratePairs(source, static (left, right) => Node.Operator.Add(left, right));
    }
}
