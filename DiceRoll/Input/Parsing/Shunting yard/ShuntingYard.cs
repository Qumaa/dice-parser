namespace DiceRoll.Input.Parsing
{
    public sealed class ShuntingYard
    {
        private readonly InfixReader _infixReader;
        private readonly PostfixEvaluator _postfixEvaluator;

        public ShuntingYard(TokensTable tokensTable, OperandCastersTable castersTable)
        {
            ShuntingYardState state = new(tokensTable);
            ShuntingYardOperators operators = new(state, castersTable);
            ShuntingYardOperands operands = new(state);
            
            _infixReader = new InfixReader(state, operators, operands);
            _postfixEvaluator = new PostfixEvaluator(state, operators, operands);
        }

        public void Append(string expression) =>
            _infixReader.Read(expression);

        public NodeTree Parse() =>
            _postfixEvaluator.Evaluate();
    }
}
