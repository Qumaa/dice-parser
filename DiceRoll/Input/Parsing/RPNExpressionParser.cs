using System;
using System.Collections.Generic;

namespace DiceRoll.Input
{
    public sealed class RPNExpressionParser
    {
        private readonly RPNTokensTable _tokensTable;
        private readonly Stack<RPNOperatorToken> _operators;
        private readonly Stack<INode> _operands;
        
        public RPNExpressionParser(RPNTokensTable rpnTokensTable) 
        {
            _tokensTable = rpnTokensTable;
            _operators = new Stack<RPNOperatorToken>();
            _operands = new Stack<INode>();
        }

        public INode Parse(ReadOnlySpan<char> expression)
        {
            VisitTableIteratively(expression);

            return CollapseParsedStack();
        }

        private void VisitTableIteratively(ReadOnlySpan<char> expression)
        {
            MatchInfo notParsed = MatchInfo.All(expression);
            
            do notParsed = VisitTableOrThrow(notParsed); 
            while (!notParsed.Empty);
        }

        private INode CollapseParsedStack()
        {
            while(_operators.TryPop(out RPNOperatorToken token))
                InvokeOperator(token.Invoker);
            
            return _operands.Pop();
        }

        private MatchInfo VisitTableOrThrow(MatchInfo notParsed)
        {
            try
            {
                MatchInfo tokenMatch = _tokensTable.Visit(new TableVisitor(this), notParsed.SliceMatch());
                return notParsed.MoveStart(tokenMatch.End);
            }
            catch (Exception e)
            {
                throw new RPNParserException(notParsed.MoveEnd(notParsed.Length - 1), e);
            }
        }

        private void InvokeOperator(RPNOperatorInvoker invoker)
        {
            if (_operands.Count < invoker.RequiredOperands)
                throw new OperatorInvocationException(invoker.RequiredOperands, _operands.Count);
            
            invoker.Invoke(_operands);
        }

        public readonly struct TableVisitor
        {
            private readonly RPNExpressionParser _parser;
            
            public TableVisitor(RPNExpressionParser parser) 
            {
                _parser = parser;
            }

            public void OpenParenthesis() =>
                _parser._operators.Push(RPNOperatorToken.OpenParenthesis);

            public void CloseParenthesis()
            {
                while (_parser._operators.TryPop(out RPNOperatorToken operatorToken))
                {
                    if (operatorToken.IsOpenParenthesis)
                        return;

                    _parser.InvokeOperator(operatorToken.Invoker);
                }
            
                throw new UnbalancedParenthesisException();
            }

            public void Operator(int precedence, RPNOperatorInvoker invoker)
            {
                while (_parser._operators.TryPeek(out RPNOperatorToken lastOperator) &&
                       !lastOperator.IsOpenParenthesis &&
                       precedence <= lastOperator.Precedence)
                    _parser.InvokeOperator(_parser._operators.Pop().Invoker);

                _parser._operators.Push(new RPNOperatorToken(precedence, invoker));
            }

            public void Operand(INumeric operand) =>
                _parser._operands.Push(operand);
        }
    }
}
