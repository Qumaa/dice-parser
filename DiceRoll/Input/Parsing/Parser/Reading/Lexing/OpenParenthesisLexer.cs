using System;

namespace DiceRoll.Input.Parsing.Deprecated
{
    public class OpenParenthesisLexer : Lexer
    {
        public const int DEFAULT_PRECEDENCE = 1100;
        
        private readonly ShuntingYardState _state;
        private readonly IToken _openParenthesis;
        
        public OpenParenthesisLexer(int precedence, IToken token, ShuntingYardState state) : base()
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

            _state.Operators.MapAndPush(GetOperator(), match);
            _state.Annotate().ParenthesisOpening();
            return true;
        }

        private Operator GetOperator() =>
            new(null, 0, _state.Operands.Count);
    }
}
