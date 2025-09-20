using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public sealed class ExpressionParser
    {
        private readonly ShuntingYard _nodeBuilder;

        public ExpressionParser(TokensTable diceFormulaTokens, OperandCastingTable castingTable)
        {
            _nodeBuilder = new ShuntingYard(diceFormulaTokens, castingTable);
        }

        public NodeTree Parse(string expression)
        {
            _nodeBuilder.Append(expression);
            return _nodeBuilder.Parse();
        }

        public NodeTree Parse(IEnumerable<string> expression)
        {
            foreach (string segment in expression)
                _nodeBuilder.Append(segment);

            return _nodeBuilder.Parse();
        }
    }
}
