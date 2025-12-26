using System;

namespace DiceRoll.Input.Parsing
{
    public class OpenParenthesisLexer : Lexer
    {
        private readonly EquationParserState _state;
        private readonly IToken _openParenthesis;
        
        public OpenParenthesisLexer(IToken token, EquationParserState state)
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

            _state.Lexemes.Push(OpenParenthesis.Shared, in match);
            return true;
        }
    }
}
