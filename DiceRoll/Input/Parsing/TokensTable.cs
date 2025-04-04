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

        public bool IsOpenParenthesis(ReadOnlySpan<char> token, out MatchInfo matchInfo) =>
            _openParenthesis.Matches(token, out matchInfo);

        public bool IsCloseParenthesis(ReadOnlySpan<char> token, out MatchInfo matchInfo) =>
            _closeParenthesis.Matches(token, out matchInfo);

        public bool IsOperator(ReadOnlySpan<char> token, out int precedence, out OperatorParser parser, out MatchInfo matchInfo)
        {
            foreach (TokenizedOperator tokenizedOperator in _operators)
            {
                if (!tokenizedOperator.Token.Matches(token, out matchInfo))
                    continue;

                precedence = tokenizedOperator.Precedence;
                parser = tokenizedOperator.Parser;
                return true;
            }

            precedence = 0;
            parser = null;
            matchInfo = default;
            return false;
        }

        public bool IsOperand(ReadOnlySpan<char> token, out INumeric node, out MatchInfo matchInfo)
        {
            foreach (TokenizedOperand operand in _operands)
            {
                if (!operand.Token.Matches(token, out matchInfo))
                    continue;

                node = operand.Handler.Invoke(matchInfo.SliceMatch());
                return true;
            }

            node = null;
            matchInfo = default;
            return false;
        }
    }
}
