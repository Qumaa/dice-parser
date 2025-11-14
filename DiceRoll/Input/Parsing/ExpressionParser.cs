using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public sealed class ExpressionParser
    {
        private readonly ShuntingYard _shuntingYard;

        public ExpressionParser(TokensTable tokensTable, OperandCastingTable castingTable)
        {
            ShuntingYardState state = new(castingTable);
            TokenGroupChain chain = tokensTable.ToDefaultChain(state);
            
            _shuntingYard = new ShuntingYard(state, chain);
        }

        /*
         * re: variables
         * derive ExternalTokenSolver, create it in this class and pass it down to shunting yard
         * variables will pop up as unknown tokens, at which point they can be put inside the state
         *
         * there must be developed a way to "lock" passed nodes from re-evaluating as part of expression in order for it to work
         * for example, passing a d20 named atk to "atk = 20 or atk >= 14" would evaluate the d20 2 times
         * "atk = 20" will be evaluated with a result that gets overriden by "atk >= 14"
         * so even if atk was 20, in the subsequent evaluation it is set to, say, 12
         * only the last 12 will be accessible through the evaluated node tree
         * this will lead to funny "true (12 = 20) or false (12 >= 14)" outputs
         *
         * substituting nodes with a "readonly" wrappers is not preferred, unless absolutely necessary
         * remember that these nodes are not "frozen", they are still re-evaluated along with the expression tree
         * the only restriction is that they are evaluated once despite however many times it is used
         * transient/scoped-like system?
         */
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
