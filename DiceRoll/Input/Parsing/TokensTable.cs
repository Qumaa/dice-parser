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

        public bool IsOpenParenthesis(in ReadOnlySpan<char> token) =>
            _openParenthesis.Matches(token);

        public bool IsCloseParenthesis(in ReadOnlySpan<char> token) =>
            _closeParenthesis.Matches(token);

        public bool IsOperator(in ReadOnlySpan<char> token, out int precedence, out OperatorParser parser)
        {
            for (int i = 0; i < _operators.Length; i++)
            {
                TokenizedOperator tokenizedOperator = _operators[i];
                
                if (!tokenizedOperator.Token.Matches(token))
                    continue;

                precedence = tokenizedOperator.Precedence;
                parser = tokenizedOperator.Parser;
                return true;
            }

            precedence = 0;
            parser = null;
            return false;
        }

        public bool IsOperand(in ReadOnlySpan<char> token, out INumeric node)
        {
            for (int i = 0; i < _operands.Length; i++)
            {
                TokenizedOperand operand = _operands[i];
                
                if (operand.Token.Matches(token))
                {
                    node = operand.Handler.Invoke(token);
                    return true;
                }
            }

            node = null;
            return false;
        }
    }
}
