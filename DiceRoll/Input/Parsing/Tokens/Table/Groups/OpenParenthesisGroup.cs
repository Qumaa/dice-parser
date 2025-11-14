using System;

namespace DiceRoll.Input.Parsing
{
    internal class OpenParenthesisGroup : TokenGroup
    {
        private readonly ShuntingYardOperators _operators;
        private readonly IToken _openParenthesis;
        
        public OpenParenthesisGroup(int precedence, IToken token, ShuntingYardOperators operators) : base(precedence)
        {
            ArgumentNullException.ThrowIfNull(operators);
            ArgumentNullException.ThrowIfNull(token);
            
            _operators = operators;
            _openParenthesis = token;
        }

        public override bool TryMatchStart(in Substring substring, out Substring match)
        {
            if (!_openParenthesis.MatchesStart(in substring, out match))
                return false;

            _operators.OpenParenthesis(in match);
            return true;
        }
    }
}
