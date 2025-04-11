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

        public Substring Visit(IVisitor visitor, Substring expression)
        {
            if (StartsWithOpenParenthesis(expression, out Substring tokenMatch))
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

            if (StartsWithOperator(expression, out tokenMatch, out int precedence, out ShuntingYard.OperatorInvoker invoker))
            {
                visitor.Operator(precedence, invoker);
                return tokenMatch;
            }

            tokenMatch = GetUnknownTokenSubstring(expression).Trim();
            
            visitor.UnknownToken(in tokenMatch);
            return tokenMatch;
        }

        private bool StartsWithOpenParenthesis(Substring expression, out Substring tokenMatch) =>
            _openParenthesis.Matches(expression, out tokenMatch);

        private bool StartsWithCloseParenthesis(Substring expression, out Substring tokenMatch) =>
            _closeParenthesis.Matches(expression, out tokenMatch);

        private bool StartsWithOperator(Substring expression, out Substring tokenMatch, out int precedence,
            out ShuntingYard.OperatorInvoker invoker)
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

        private bool StartsWithOperand(Substring expression, out Substring tokenMatch, out INumeric operand)
        {
            foreach (TokenizedOperand operandToken in _operands)
            {
                if (!operandToken.Token.Matches(expression, out tokenMatch))
                    continue;

                operand = operandToken.Parse(tokenMatch);
                return true;
            }

            operand = null;
            tokenMatch = default;
            return false;
        }

        private Substring GetUnknownTokenSubstring(Substring expression)
        {
            for (int i = 0; i < expression.Length; i++)
                if (_MatchesAnyToken(expression[i..]))
                    return expression.SetLength(i);

            return expression;

            bool _MatchesAnyToken(Substring expression)
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

            void Operator(int precedence, ShuntingYard.OperatorInvoker invoker);

            void Operand(INumeric operand);

            void UnknownToken(in Substring tokenMatch);
        }
    }
}
