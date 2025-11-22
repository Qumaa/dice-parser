using System;

namespace DiceRoll.Input.Parsing.Deprecated
{
    public class CloseParenthesisLexer : Lexer
    {
        public const int DEFAULT_PRECEDENCE = 1000;
        
        private readonly ShuntingYardState _state;
        private readonly IToken _closeParenthesis;

        public CloseParenthesisLexer(int precedence, IToken token, ShuntingYardState state) : base()
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
            
            InvokeOperatorsUntilOpenParenthesis(out Mapped<Operator> openParenthesis);
            
            GroupIntoPoolIfNeeded(in openParenthesis, in match);
            
            _state.Annotate().ParenthesisClosing();
            
            return true;
        }

        private void InvokeOperatorsUntilOpenParenthesis(out Mapped<Operator> openParenthesis)
        {
            while (_state.Operators.TryPop(out Mapped<Operator> @operator))
            {
                if (!@operator.Value.IsOpenParenthesis)
                {
                    _state.InvocationHandler.InvokeOperator(in @operator);
                    continue;
                }

                openParenthesis = @operator;
                return;
            }
            
            throw new UnbalancedParenthesisException();
        }

        private void GroupIntoPoolIfNeeded(in Mapped<Operator> openParenthesis, in Substring closeParenthesis)
        {
            int produced = _state.Operands.Count - openParenthesis.Value.Position;
                
            if (produced is 1)
                return;
                
            Mapped<LinkedNode> pool = NodePoolUtils.GroupNodes(_state.Operands.PopMany(produced));
            
            Range withParenthesis = pool.Range.And(openParenthesis.Range).And(closeParenthesis.AsRange());
            
            pool = new Mapped<LinkedNode>(in pool.Value, in withParenthesis);
            
            _state.Operands.Push(in pool);
        }
    }
}
