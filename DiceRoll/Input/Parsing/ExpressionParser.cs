using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public sealed class ExpressionParser
    {
        private readonly ShuntingYard _shuntingYard;

        public ExpressionParser(TokensTable tokensTable, OperandCastingTable castingTable)
        {
            _shuntingYard = new ShuntingYard(tokensTable, castingTable);
        }

        public NodeTree Parse(string expression)
        {
            _shuntingYard.Append(expression);
            return _shuntingYard.Parse();
        }

        public NodeTree Parse(IEnumerable<string> expression)
        {
            foreach (string segment in expression)
                _shuntingYard.Append(segment);

            return _shuntingYard.Parse();
        }
    }
}
