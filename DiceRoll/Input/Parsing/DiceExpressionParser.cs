using System;
using System.Collections.Generic;

namespace DiceRoll.Input
{
    public sealed class DiceExpressionParser
    {
        private readonly TokensTable _tokensTable;
        private readonly Stack<RPNOperatorToken> _operators;
        private readonly Stack<INode> _operands;
        private readonly Stack<RPNOperatorInvoker> _delayedInvokers;
        
        public DiceExpressionParser(TokensTable tokensTable) 
        {
            _tokensTable = tokensTable;
            _operators = new Stack<RPNOperatorToken>();
            _operands = new Stack<INode>();
            _delayedInvokers = new Stack<RPNOperatorInvoker>();
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
            
            InvokeDelayedOperators();
            
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
                throw new DiceExpressionParsingException(notParsed.MoveEnd(notParsed.Length - 1), e);
            }
        }

        private void InvokeOperator(RPNOperatorInvoker invoker)
        {
            if (_operands.Count < invoker.RequiredOperands)
                throw new OperatorInvocationException(invoker.RequiredOperands, _operands.Count);
            
            invoker.Invoke(new OperandsStackAccess(this));
        }

        private void InvokeDelayedOperators()
        {
            while(_delayedInvokers.TryPop(out RPNOperatorInvoker invoker))
                InvokeOperator(invoker);
        }

        private void InvokeDelayedOperatorsAfter(RPNOperatorInvoker invoker)
        {
            InvokeOperator(invoker);
            InvokeDelayedOperators();
        }

        public readonly struct TableVisitor
        {
            private readonly DiceExpressionParser _parser;
            
            public TableVisitor(DiceExpressionParser parser) 
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

                    _parser.InvokeDelayedOperatorsAfter(operatorToken.Invoker);
                }
            
                throw new UnbalancedParenthesisException();
            }

            public void Operator(int precedence, RPNOperatorInvoker invoker)
            {
                while (_parser._operators.TryPeek(out RPNOperatorToken lastOperator) &&
                       !lastOperator.IsOpenParenthesis &&
                       precedence < lastOperator.Precedence)
                    _parser.InvokeDelayedOperatorsAfter(_parser._operators.Pop().Invoker);

                _parser._operators.Push(new RPNOperatorToken(precedence, invoker));
            }

            public void Operand(INumeric operand)
            {
                _parser._operands.Push(operand);
                
                _parser.InvokeDelayedOperators();
            }
        }

        public readonly struct OperandsStackAccess
        {
            private readonly DiceExpressionParser _parser;

            public OperandsStackAccess(DiceExpressionParser parser) 
            {
                _parser = parser;
            }

            public T Pop<T>() where T : INode
            {
                INode node = _parser._operands.Pop();
                
                return node is T operand ?
                    operand :
                    throw new OperatorInvocationException(typeof(T), node.GetType());
            }

            public void Push(INode operand) =>
                _parser._operands.Push(operand);

            public void ForNextOperand(RPNOperatorInvoker invoker) =>
                _parser._delayedInvokers.Push(invoker);
        }
    }
}
