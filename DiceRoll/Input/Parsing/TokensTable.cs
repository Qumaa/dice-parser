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
            _openParenthesis.MatchesStart(in expression, out tokenMatch);

        public bool StartsWithCloseParenthesis(in Substring expression, out Substring tokenMatch) =>
            _closeParenthesis.MatchesStart(in expression, out tokenMatch);

        public bool StartsWithOperator(in Substring expression, OperatorKind kind, out Substring tokenMatch, out int precedence,
            out OperatorInvoker invoker) =>
            StartsWithOperator(in expression, OperatorKindToArity(kind), out tokenMatch, out precedence, out invoker);

        public bool StartsWithOperand(in Substring expression, out Substring tokenMatch, out INumeric operand)
        {
            foreach (TokenizedOperand operandToken in _operands)
            {
                if (!operandToken.Token.MatchesStart(in expression, out tokenMatch))
                    continue;

                operand = operandToken.Parse(tokenMatch);
                return true;
            }

            operand = null;
            tokenMatch = default;
            return false;
        }

        public Substring UntilFirstKnownToken(in Substring expression, OperatorKind ignoreOperatorKind)
        {
            return _MatchesAnyToken(in expression, OperatorKindToArity(ignoreOperatorKind), out Substring match) ?
                expression.SetLength(match.Start - expression.Start) :
                expression;

            bool _MatchesAnyToken(in Substring expression, int ignoreArity, out Substring match)
            {
                if (_openParenthesis.Matches(in expression, out match))
                    return true;
                
                if (_closeParenthesis.Matches(in expression, out match))
                    return true;

                foreach (TokenizedOperator tokenizedOperator in _operators)
                    if (tokenizedOperator.Invoker.Arity != ignoreArity && tokenizedOperator.Token.Matches(in expression, out match))
                        return true;
                
                foreach (TokenizedOperand tokenizedOperand in _operands)
                    if (tokenizedOperand.Token.Matches(in expression, out match))
                        return true;

                return false;
            }
        }

        private bool StartsWithOperator(in Substring expression, int arity, out Substring tokenMatch, out int precedence,
            out OperatorInvoker invoker)
        {
            foreach (TokenizedOperator tokenizedOperator in _operators)
            {
                if (tokenizedOperator.Invoker.Arity != arity ||
                    !tokenizedOperator.Token.MatchesStart(in expression, out tokenMatch))
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

        private static int OperatorKindToArity(OperatorKind kind) =>
            kind switch
            {
                OperatorKind.Unary => 1,
                OperatorKind.Binary => 2
            };
    }
}
