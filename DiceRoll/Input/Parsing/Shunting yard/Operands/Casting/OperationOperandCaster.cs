namespace DiceRoll.Input.Parsing
{
    public sealed class OperationOperandCaster : OperandCaster<IOperation, IAssertion>
    {
        protected override IAssertion Cast(IOperation source) =>
            source.AsAssertion;
    }
}
