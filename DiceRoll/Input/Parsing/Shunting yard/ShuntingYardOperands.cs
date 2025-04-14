namespace DiceRoll.Input
{
    internal sealed class ShuntingYardOperands
    {
        private readonly ShuntingYardState _state;

        public ShuntingYardOperands(ShuntingYardState state)
        {
            _state = state;
        }

        public void Push(INumeric operand, in Substring context) =>
            _state.Operands.Push(operand, in context);

        public bool TryPeek(out FormulaToken<INode> formulaToken) =>
            _state.Operands.TryPeek(out formulaToken);

        public INode Pop() =>
            _state.Operands.PopValue();
    }
}
