using System.Collections.Generic;

namespace DiceRoll.Input
{
    public class RPNBuilder
    {
        private readonly Stack<RPNOperator> _operators = new();
        private readonly List<string> _output = new();
        
        private readonly TokensTable _tokensTable;
        
        public RPNBuilder(TokensTable tokensTable) 
        {
            _tokensTable = tokensTable;
        }

        public void Push(in string token)
        {
            if (TryOpenParenthesis(token))
                return;

            if (TryCloseParenthesis(token))
                return;
            
            if (TryAsOperator(token))
                return;
            
            if (TryAsOperand(token))
                return;

            throw new UnknownTokenException();
        }

        public List<string> Build()
        {
            while(_operators.TryPop(out RPNOperator rpnOperator))
                Append(rpnOperator.Token);
            
            return _output;
        }

        private bool TryOpenParenthesis(in string token)
        {
            if (!_tokensTable.IsOpenParenthesis(token))
                return false;
            
            Push(new RPNOperator(token));
            return true;
        }

        private bool TryCloseParenthesis(in string token)
        {
            if (!_tokensTable.IsCloseParenthesis(token))
                return false;
            
            while (_operators.TryPop(out RPNOperator rpnOperator))
            {
                if (_tokensTable.IsOpenParenthesis(rpnOperator.Token))
                    return true;
                
                Append(rpnOperator.Token);
            }
            
            throw new UnbalancedParenthesisException();
        }

        private bool TryAsOperator(in string token)
        {
            if (!_tokensTable.IsOperator(token, out int precedence))
                return false;

            while (_operators.TryPeek(out RPNOperator lastOperator) &&
                   !_tokensTable.IsOpenParenthesis(lastOperator.Token) &&
                   !LeftPrecedenceIsHigher(precedence, lastOperator.Precedence))
                Append(_operators.Pop().Token);
            
            Push(new RPNOperator(token, precedence));
            return true;
        }

        private bool TryAsOperand(in string token)
        {
            if (!_tokensTable.IsOperand(token))
                return false;
            
            Append(token);
            return true;
        }

        private void Append(string token) =>
            _output.Add(token);

        private void Push(in RPNOperator rpnOperator) =>
            _operators.Push(rpnOperator);

        private static bool LeftPrecedenceIsHigher(int left, int right) =>
            left > right;
    }
}
