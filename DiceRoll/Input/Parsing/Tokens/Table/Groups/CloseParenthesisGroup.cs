using System;

namespace DiceRoll.Input.Parsing
{
    public class CloseParenthesisGroup : TokenGroup
    {
        public const int DEFAULT_PRECEDENCE = 1000;
        
        private readonly ShuntingYardState _state;
        private readonly IToken _closeParenthesis;

        public CloseParenthesisGroup(int precedence, IToken token, ShuntingYardState state) : base(precedence)
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
            
            ThrowIfImbalanced();

            InvokeOperatorsUntilOpenParenthesis();
            
            _state.Annotate().ParenthesisClosing();
            
            return true;
        }

        private void InvokeOperatorsUntilOpenParenthesis()
        {
            while (_state.Operators.TryPop(out Mapped<Operator> @operator))
            {
                if (@operator.Value.IsOpenParenthesis)
                    break;

                _state.InvocationHandler.InvokeOperator(in @operator);
            }
        }

        private void ThrowIfImbalanced()
        {
            if (_state.ParenthesisLevel <= 0)
                throw new UnbalancedParenthesisException();
        }
    }
}
