namespace DiceRoll.Input
{
    public sealed class ShuntingYard
    {
        private readonly InfixReader _infixReader;
        private readonly PostfixEvaluator _postfixEvaluator;

        public ShuntingYard(TokensTable tokensTable)
        {
            ShuntingYardState state = new(tokensTable);
            ShuntingYardOperators operators = new(state);
            ShuntingYardOperands operands = new(state);
            
            _infixReader = new InfixReader(state, operators, operands);
            _postfixEvaluator = new PostfixEvaluator(state, operators, operands);
        }

        public void Append(string expression) =>
            _infixReader.Read(expression);

        public INode Parse() =>
            _postfixEvaluator.Evaluate();
    }
}
