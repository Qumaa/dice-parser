namespace DiceRoll.Input.Parsing
{
    internal sealed class ShuntingYardOperands
    {
        private readonly ShuntingYardState _state;

        public ShuntingYardOperands(ShuntingYardState state)
        {
            _state = state;
        }

        public void PushParentless(INumeric operand, in Substring context) =>
            _state.Operands.MapAndPush(new LinkedNode(operand), in context);

        public bool TryPeek(out Mapped<LinkedNode> mapped) =>
            _state.Operands.TryPeek(out mapped);

        public Mapped<LinkedNode> Pop() =>
            _state.Operands.Pop();
    }
}
