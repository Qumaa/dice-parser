using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class ShuntingYard
    {
        private readonly InfixReader _infixReader;
        private readonly PostfixEvaluator _postfixEvaluator;

        public ShuntingYard(TokensTable tokensTable, OperandCastingTable castingTable)
        {
            ArgumentNullException.ThrowIfNull(tokensTable);
            ArgumentNullException.ThrowIfNull(castingTable);
            
            ShuntingYardState state = new(tokensTable);
            ShuntingYardOperators operators = new(state, castingTable);
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
