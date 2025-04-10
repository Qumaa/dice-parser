using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input
{
    public class TokensTable
    {
        private readonly IToken _openParenthesis;
        private readonly IToken _closeParenthesis;
        private readonly TokenizedOperator[] _operators;
        private readonly TokenizedOperand[] _operands;

        public TokensTable(IToken openParenthesis, IToken closeParenthesis, IEnumerable<TokenizedOperator> operators,
            IEnumerable<TokenizedOperand> operands)
        {
            _openParenthesis = openParenthesis;
            _closeParenthesis = closeParenthesis;
            
            _operators = operators.ToArray();
            _operands = operands.ToArray();
        }

        public MatchInfo Visit(IVisitor visitor, ReadOnlySpan<char> expression)
        {
            if (StartsWithOpenParenthesis(expression, out MatchInfo tokenMatch))
            {
                visitor.OpenParenthesis();
                return tokenMatch;
            }

            if (StartsWithCloseParenthesis(expression, out tokenMatch))
            {
                visitor.CloseParenthesis();
                return tokenMatch;
            }
            
            if (StartsWithOperand(expression, out tokenMatch, out INumeric operand))
            {
                visitor.Operand(operand);
                return tokenMatch;
            }

            if (StartsWithOperator(expression, out tokenMatch, out int precedence, out RPNOperatorInvoker invoker))
            {
                visitor.Operator(precedence, invoker);
                return tokenMatch;
            }

            int length = GetUnknownTokenLength(expression);
            tokenMatch = new MatchInfo(expression, 0, length).Trim();
            
            visitor.UnknownToken(in tokenMatch);
            return tokenMatch;
        }

        private bool StartsWithOpenParenthesis(ReadOnlySpan<char> expression, out MatchInfo tokenMatch) =>
            _openParenthesis.Matches(expression, out tokenMatch);

        private bool StartsWithCloseParenthesis(ReadOnlySpan<char> expression, out MatchInfo tokenMatch) =>
            _closeParenthesis.Matches(expression, out tokenMatch);

        private bool StartsWithOperator(ReadOnlySpan<char> expression, out MatchInfo tokenMatch, out int precedence,
            out RPNOperatorInvoker invoker)
        {
            foreach (TokenizedOperator tokenizedOperator in _operators)
            {
                if (!tokenizedOperator.Token.Matches(expression, out tokenMatch))
                    continue;

                precedence = tokenizedOperator.Precedence;
                invoker = tokenizedOperator.Invoker;
                return true;
            }

            precedence = 0;
            invoker = null;
            tokenMatch = default;
            return false;
        }

        private bool StartsWithOperand(ReadOnlySpan<char> expression, out MatchInfo tokenMatch, out INumeric operand)
        {
            foreach (TokenizedOperand operandToken in _operands)
            {
                if (!operandToken.Token.Matches(expression, out tokenMatch))
                    continue;

                operand = operandToken.Handler.Invoke(tokenMatch.SliceMatch());
                return true;
            }

            operand = null;
            tokenMatch = default;
            return false;
        }

        private int GetUnknownTokenLength(in ReadOnlySpan<char> expression)
        {
            for (int i = 0; i < expression.Length; i++)
                if (_MatchesAnyToken(expression[i..]))
                    return i;

            return expression.Length;

            bool _MatchesAnyToken(ReadOnlySpan<char> expression)
            {
                if (_openParenthesis.Matches(expression))
                    return true;
                
                if (_closeParenthesis.Matches(expression))
                    return true;

                foreach (TokenizedOperator tokenizedOperator in _operators)
                    if (tokenizedOperator.Token.Matches(expression))
                        return true;
                
                foreach (TokenizedOperand tokenizedOperand in _operands)
                    if (tokenizedOperand.Token.Matches(expression))
                        return true;

                return false;
            }
        }

        public interface IVisitor
        { 
            void OpenParenthesis();

            void CloseParenthesis();

            void Operator(int precedence, RPNOperatorInvoker invoker);

            void Operand(INumeric operand);

            void UnknownToken(in MatchInfo tokenMatch);
        }
    }
}
