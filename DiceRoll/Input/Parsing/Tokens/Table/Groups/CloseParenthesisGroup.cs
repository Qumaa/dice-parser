using System;

namespace DiceRoll.Input.Parsing
{
    internal class CloseParenthesisGroup : TokenGroup
    {
        private readonly ShuntingYardOperators _operators;
        private readonly IToken _closeParenthesis;

        public CloseParenthesisGroup(int precedence, IToken token, ShuntingYardOperators operators) : base(precedence)
        {
            ArgumentNullException.ThrowIfNull(operators);
            ArgumentNullException.ThrowIfNull(token);

            _operators = operators;
            _closeParenthesis = token;
        }

        public override bool TryMatch(in Substring substring, out Substring match)
        {
            if (!_closeParenthesis.MatchesStart(in substring, out match))
                return false;
            
            _operators.CloseParenthesis();
            return true;
        }
    }
}
