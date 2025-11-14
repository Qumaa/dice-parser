using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class ShuntingYard
    {
        private readonly InfixReader _infixReader;
        private readonly PostfixEvaluator _postfixEvaluator;

        public ShuntingYard(ShuntingYardState state, TokenGroupChain chain)
        {
            ArgumentNullException.ThrowIfNull(state);
            ArgumentNullException.ThrowIfNull(chain);
            
            _infixReader = new InfixReader(state, chain);
            _postfixEvaluator = new PostfixEvaluator(state);
        }

        public void Append(string expression, ExternalTokenSolver solver) =>
            _infixReader.Read(expression, solver);

        public NodeTree Parse() =>
            _postfixEvaluator.Evaluate();
    }

    public static class ShuntingYardExtensions
    {
        public static void Append(this ShuntingYard yard, string expression) =>
            yard.Append(expression, ExternalTokenSolver.Inert);
    }
}
