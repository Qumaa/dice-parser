namespace DiceRoll.Input.Parsing
{
    public sealed class CompositeOperandCaster : OperandCaster<IComposite, INumeric>
    {
        protected override bool TryCast(IComposite source, out INumeric result) =>
            (result = source.AsNumeric) is not null;
    }
}
