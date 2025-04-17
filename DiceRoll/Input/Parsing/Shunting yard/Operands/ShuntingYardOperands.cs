namespace DiceRoll.Input.Parsing
{
    internal sealed class ShuntingYardOperands
    {
        private readonly ShuntingYardState _state;

        public ShuntingYardOperands(ShuntingYardState state)
        {
            _state = state;
        }

        public void Push(INumeric operand, in Substring context) =>
            _state.Operands.MapAndPush(operand, in context);

        public bool TryPeek(out Mapped<INode> mapped) =>
            _state.Operands.TryPeek(out mapped);

        public INode Pop() =>
            _state.Operands.PopValue();
    }
}
