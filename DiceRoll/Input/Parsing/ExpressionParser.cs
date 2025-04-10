namespace DiceRoll.Input
{
    // for the time being, this is a simple shunting-yard wrapper. A useless one, even
    public sealed class ExpressionParser
    {
        private readonly ShuntingYard _nodeBuilder;

        public ExpressionParser(TokensTable diceFormulaTokens)
        {
            _nodeBuilder = new ShuntingYard(diceFormulaTokens);
        }

        public INode Parse(string expression)
        {
                _nodeBuilder.Push(expression);
                return _nodeBuilder.Build();
        }

        public INode Parse(string[] expression)
        {
            foreach (string segment in expression)
                _nodeBuilder.Push(segment);

            return _nodeBuilder.Build();
        }
    }
}
