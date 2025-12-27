namespace DiceRoll.Input.Parsing
{
    public sealed class OperationOperandCaster : OperandCaster<IOperation, IAssertion>
    {
        protected override bool TryCast(IOperation source, out IAssertion result) =>
            (result = source.AsAssertion) is not null;
    }
}
