namespace DiceRoll.Input.Parsing
{
    public sealed class CompositeOperandCaster : OperandCaster<IComposite, INumeric>
    {
        protected override INumeric Cast(IComposite source) =>
            source.AsNumeric;
    }
}
