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

        public bool IsOpenParenthesis(string token) =>
            _openParenthesis.Matches(token);

        public bool IsCloseParenthesis(string token) =>
            _closeParenthesis.Matches(token);

        public bool IsOperator(string token, out int precedence)
        {
            for (int i = 0; i < _operators.Length; i++)
            {
                TokenizedOperator tokenizedOperator = _operators[i];
                
                if (!tokenizedOperator.Token.Matches(token))
                    continue;

                precedence = tokenizedOperator.Precedence;
                return true;
            }

            precedence = 0;
            return false;
        }

        public bool IsOperand(string token)
        {
            for (int i = 0; i < _operands.Length; i++)
                if (_operands[i].Token.Matches(token))
                    return true;

            return false;
        }
    }
}
