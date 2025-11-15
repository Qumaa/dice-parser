namespace DiceRoll.Input.Parsing
{
    // todo inline the operator invocation struct into this class
    public sealed class OperatorInvocationHandler
    {
        private readonly ShuntingYardState _state;
        private readonly OperandCastingTable _castingTable;
        
        public OperatorInvocationHandler(ShuntingYardState state, OperandCastingTable castingTable)
        {
            _state = state;
            _castingTable = castingTable;
        }

        public void InvokeOperator(in Mapped<Operator> operatorToken) =>
            new OperatorInvocation(_state, _castingTable, operatorToken.Value.InvocationBehaviour, in operatorToken.Range).Perform();
    }
}
