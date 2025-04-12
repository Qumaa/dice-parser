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

        public bool StartsWithOpenParenthesis(in Substring expression, out Substring tokenMatch) =>
            _openParenthesis.Matches(in expression, out tokenMatch);

        public bool StartsWithCloseParenthesis(in Substring expression, out Substring tokenMatch) =>
            _closeParenthesis.Matches(in expression, out tokenMatch);

        public bool StartsWithOperator(in Substring expression, out Substring tokenMatch, out int precedence,
            out OperatorInvoker invoker)
        {
            foreach (TokenizedOperator tokenizedOperator in _operators)
            {
                if (!tokenizedOperator.Token.Matches(in expression, out tokenMatch))
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

        public bool StartsWithOperand(in Substring expression, out Substring tokenMatch, out INumeric operand)
        {
            foreach (TokenizedOperand operandToken in _operands)
            {
                if (!operandToken.Token.Matches(in expression, out tokenMatch))
                    continue;

                operand = operandToken.Parse(tokenMatch);
                return true;
            }

            operand = null;
            tokenMatch = default;
            return false;
        }

        public Substring UntilFirstKnownToken(in Substring expression)
        {
            for (int i = 0; i < expression.Length; i++)
                if (_MatchesAnyToken(expression[i..]))
                    return expression.SetLength(i);

            return expression;

            bool _MatchesAnyToken(Substring expression)
            {
                if (_openParenthesis.Matches(in expression))
                    return true;
                
                if (_closeParenthesis.Matches(in expression))
                    return true;

                foreach (TokenizedOperator tokenizedOperator in _operators)
                    if (tokenizedOperator.Token.Matches(in expression))
                        return true;
                
                foreach (TokenizedOperand tokenizedOperand in _operands)
                    if (tokenizedOperand.Token.Matches(in expression))
                        return true;

                return false;
            }
        }
    }
}
