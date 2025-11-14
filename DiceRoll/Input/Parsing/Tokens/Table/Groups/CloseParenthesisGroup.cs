using System;

namespace DiceRoll.Input.Parsing
{
    internal class CloseParenthesisGroup : TokenGroup
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
            
            if (_state.ClosingParenthesisWouldImposeImbalance)
                throw new UnbalancedParenthesisException();

            while (_state.Operators.TryPop(out Mapped<Operator> operatorToken))
            {
                if (operatorToken.Value.IsOpenParenthesis)
                    break;

                _state.InvocationHandler.InvokeOperator(in operatorToken);
            }
            
            _state.Annotate().ParenthesisClosing();
            
            _state.InvocationHandler.TryInvokeDelayedOperators();
            
            return true;
        }
    }
}
