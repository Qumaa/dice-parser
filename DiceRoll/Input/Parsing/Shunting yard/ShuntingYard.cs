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
            
            ShuntingYardState state = new();
            ShuntingYardOperators operators = new(state, castingTable);
            ShuntingYardOperands operands = new(state);
            
            _infixReader = new InfixReader(state, CreateChain(tokensTable, operators, operands));
            _postfixEvaluator = new PostfixEvaluator(state, operators, operands);
        }

        private static TokenGroupChain CreateChain(TokensTable tokensTable, ShuntingYardOperators operators,
            ShuntingYardOperands operands) =>
            new(new TokenGroup[]
                {
                    new OpenParenthesisGroup(100, tokensTable.OpenParenthesis, operators),
                    new OpenParenthesisGroup(90, tokensTable.CloseParenthesis, operators),
                    new OperandGroup(50, tokensTable.Operands, operands, operators),
                    new OperatorGroup(0, tokensTable.Operators, operators)
                }
                );

        public void Append(string expression) =>
            _infixReader.Read(expression);

        public NodeTree Parse() =>
            _postfixEvaluator.Evaluate();
    }
}
