using System;

namespace DiceRoll.Input.Parsing
{
    public class CloseParenthesisLexer : Lexer
    {
        private readonly LexemesList _lexemes;
        private readonly IToken _closeParenthesis;

        public CloseParenthesisLexer(IToken token, LexemesList lexemes)
        {
            ArgumentNullException.ThrowIfNull(lexemes);
            ArgumentNullException.ThrowIfNull(token);

            _closeParenthesis = token;
            _lexemes = lexemes;
        }

        public override bool TryExecute(in Substring substring, out Substring match)
        {
            if (!_closeParenthesis.MatchesStart(in substring, out match))
                return false;
            
            _lexemes.Push(CloseParenthesis.Shared, in match);
            
            return true;
        }
    }
}
