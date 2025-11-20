using System;

namespace DiceRoll.Input.Parsing
{
    public class CloseParenthesisGroup : TokenGroup
    {
        public const int DEFAULT_PRECEDENCE = 1000;
        
        private readonly EquationParserState _state;
        private readonly IToken _closeParenthesis;

        public CloseParenthesisGroup(int precedence, IToken token, EquationParserState state) : base(precedence)
        {
            ArgumentNullException.ThrowIfNull(state);
            ArgumentNullException.ThrowIfNull(token);

            _state = state;
            _closeParenthesis = token;
        }

        public override bool TryExecute(in Substring substring, out Substring match)
        {
            if (!_closeParenthesis.MatchesStart(in substring, out match))
                return false;
            
            _state.Members.Push(CloseParenthesis.Shared, in match);
            
            return true;
        }
    }
}
