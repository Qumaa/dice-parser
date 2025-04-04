using System;
using System.Collections.Generic;

namespace DiceRoll.Input
{
    public sealed class RPNExpressionBuilder
    {
        private readonly TokensTable _tokensTable;
        private readonly Stack<RPNOperatorToken> _operators;
        private readonly Stack<INode> _operands;
        
        public RPNExpressionBuilder(TokensTable tokensTable) 
        {
            _tokensTable = tokensTable;
            _operators = new Stack<RPNOperatorToken>();
            _operands = new Stack<INode>();
        }

        public void Push(in ReadOnlySpan<char> token)
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

        public void Push(string token) =>
            Push(token.AsSpan());

        public INode Build()
        {
            while(_operators.TryPop(out RPNOperatorToken token))
                ApplyOperator(token.Parser);
            
            return _operands.Pop();
        }
        
        private bool TryOpenParenthesis(in ReadOnlySpan<char> token)
        {
            if (!_tokensTable.IsOpenParenthesis(in token))
                return false;
            
            _operators.Push(RPNOperatorToken.OpenParenthesis);
            return true;
        }
        
        private bool TryCloseParenthesis(in ReadOnlySpan<char> token)
        {
            if (!_tokensTable.IsCloseParenthesis(token))
                return false;
            
            while (_operators.TryPop(out RPNOperatorToken operatorToken))
            {
                if (operatorToken.IsOpenParenthesis)
                    return true;
                
                ApplyOperator(operatorToken.Parser);
            }
            
            throw new UnbalancedParenthesisException();
        }
        
        private bool TryAsOperator(in ReadOnlySpan<char> token)
        {
            if (!_tokensTable.IsOperator(in token, out int precedence, out RPNOperatorParser parser))
                return false;

            while (_operators.TryPeek(out RPNOperatorToken lastOperator) &&
                   !lastOperator.IsOpenParenthesis &&
                   precedence <= lastOperator.Precedence)
                ApplyOperator(_operators.Pop().Parser);
            
            _operators.Push(new RPNOperatorToken(precedence, parser));
            return true;
        }
        
        private bool TryAsOperand(in ReadOnlySpan<char> token)
        {
            if (!_tokensTable.IsOperand(in token, out INumeric node))
                return false;
            
            _operands.Push(node);
            return true;
        }

        private void ApplyOperator(RPNOperatorParser parser)
        {
            if (_operands.Count < parser.RequiredOperands)
                throw new Exception(); // todo
            
            parser.TransformOperands(_operands);
        }
    }
}
