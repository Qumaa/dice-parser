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
            
            ShuntingYardState state = new(castingTable);

            _infixReader = new InfixReader(state, tokensTable.ToDefaultChain(state));
            _postfixEvaluator = new PostfixEvaluator(state);
        }

        public void Append(string expression) =>
            _infixReader.Read(expression);

        public NodeTree Parse() =>
            _postfixEvaluator.Evaluate();
    }
}
