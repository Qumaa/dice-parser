using System;

namespace DiceRoll.Input.Parsing
{
    public class OpenParenthesisLexer : Lexer
    {
        private readonly LexemesList _lexemes;
        private readonly IToken _openParenthesis;
        
        public OpenParenthesisLexer(IToken token, LexemesList lexemes)
        {
            ArgumentNullException.ThrowIfNull(lexemes);
            ArgumentNullException.ThrowIfNull(token);
            
            _lexemes = lexemes;
            _openParenthesis = token;
        }

        public override bool TryExecute(in Substring substring, Cursor cursor, out Substring match)
        {
            if (!_openParenthesis.MatchesStart(in substring, out match))
                return false;

            cursor.MoveTo(match.AsRange());
            
            _lexemes.Push(OpenParenthesis.Shared, in match);
            
            cursor.MoveToPrevious();
            
            return true;
        }
    }
}
