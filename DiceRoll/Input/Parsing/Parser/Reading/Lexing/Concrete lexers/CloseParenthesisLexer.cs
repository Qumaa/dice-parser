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

        public override bool TryExecute(in Substring substring, Cursor cursor, out Substring match)
        {
            if (!_closeParenthesis.MatchesStart(in substring, out match))
                return false;
            
            cursor.MoveTo(match.AsRange());
            
            _lexemes.Push(CloseParenthesis.Shared, in match);
            
            cursor.MoveToPrevious();
            
            return true;
        }
    }
}
