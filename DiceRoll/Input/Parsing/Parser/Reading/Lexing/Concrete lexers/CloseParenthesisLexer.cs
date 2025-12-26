using System;

namespace DiceRoll.Input.Parsing
{
    public class CloseParenthesisLexer : Lexer
    {
        private readonly EquationParserState _state;
        private readonly IToken _closeParenthesis;

        public CloseParenthesisLexer(IToken token, EquationParserState state)
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
            
            _state.Lexemes.Push(CloseParenthesis.Shared, in match);
            
            return true;
        }
    }
}
