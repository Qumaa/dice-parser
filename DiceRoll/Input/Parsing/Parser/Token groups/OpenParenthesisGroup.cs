using System;

namespace DiceRoll.Input.Parsing
{
    public class OpenParenthesisGroup : TokenGroup
    {
        public const int DEFAULT_PRECEDENCE = 1100;
        
        private readonly EquationParserState _state;
        private readonly IToken _openParenthesis;
        
        public OpenParenthesisGroup(int precedence, IToken token, EquationParserState state) : base(precedence)
        {
            ArgumentNullException.ThrowIfNull(state);
            ArgumentNullException.ThrowIfNull(token);
            
            _state = state;
            _openParenthesis = token;
        }

        public override bool TryExecute(in Substring substring, out Substring match)
        {
            if (!_openParenthesis.MatchesStart(in substring, out match))
                return false;

            _state.Members.Push(OpenParenthesis.Shared, in match);
            return true;
        }
    }
}
