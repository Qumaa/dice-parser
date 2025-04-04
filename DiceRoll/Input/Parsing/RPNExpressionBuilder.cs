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

        public void Push(ReadOnlySpan<char> token)
        {
            MatchInfo matchInfo = new(token, 0 ,0);
            
            do
            {
                if (TryOpenParenthesis(matchInfo.SliceRest(), ref matchInfo))
                    continue;

                if (TryCloseParenthesis(matchInfo.SliceRest(), ref matchInfo))
                    continue;
            
                if (TryAsOperator(matchInfo.SliceRest(), ref matchInfo))
                    continue;
            
                if (TryAsOperand(matchInfo.SliceRest(), ref matchInfo))
                    continue;

                throw new UnknownTokenException(token);
            } while (!matchInfo.SourceEndsWithThis);
        }

        public void Push(string token) =>
            Push(token.AsSpan());

        public INode Build()
        {
            while(_operators.TryPop(out RPNOperatorToken token))
                ApplyOperator(token.Parser);
            
            return _operands.Pop();
        }
        
        private bool TryOpenParenthesis(ReadOnlySpan<char> token, ref MatchInfo matchInfo)
        {
            if (!_tokensTable.IsOpenParenthesis(token, out MatchInfo successfulMatchInfo))
                return false;

            _operators.Push(RPNOperatorToken.OpenParenthesis);
            matchInfo = successfulMatchInfo;
            return true;
        }
        
        private bool TryCloseParenthesis(ReadOnlySpan<char> token, ref MatchInfo matchInfo)
        {
            if (!_tokensTable.IsCloseParenthesis(token, out MatchInfo successfulMatchInfo))
                return false;

            while (_operators.TryPop(out RPNOperatorToken operatorToken))
            {
                if (operatorToken.IsOpenParenthesis)
                {
                    matchInfo = successfulMatchInfo;
                    return true;
                }

                ApplyOperator(operatorToken.Parser);
            }
            
            throw new UnbalancedParenthesisException();
        }
        
        private bool TryAsOperator(ReadOnlySpan<char> token, ref MatchInfo matchInfo)
        {
            if (!_tokensTable.IsOperator(token, out int precedence, out OperatorParser parser, out MatchInfo successfulMatchInfo))
                return false;

            while (_operators.TryPeek(out RPNOperatorToken lastOperator) &&
                   !lastOperator.IsOpenParenthesis &&
                   precedence <= lastOperator.Precedence)
                ApplyOperator(_operators.Pop().Parser);
            
            _operators.Push(new RPNOperatorToken(precedence, parser));
            matchInfo = successfulMatchInfo;
            return true;
        }
        
        private bool TryAsOperand(ReadOnlySpan<char> token, ref MatchInfo matchInfo)
        {
            if (!_tokensTable.IsOperand(token, out INumeric node, out MatchInfo successfulMatchInfo))
                return false;
            
            _operands.Push(node);
            matchInfo = successfulMatchInfo;
            return true;
        }

        private void ApplyOperator(OperatorParser parser)
        {
            if (_operands.Count < parser.RequiredOperands)
                throw new UnbalancedParenthesisException();
            
            parser.TransformOperands(_operands);
        }
    }
}
