using System;
using System.Collections.Generic;

namespace DiceRoll.Input
{
    public sealed class ShuntingYard
    {
        private readonly TokensTable _tokensTable;
        private readonly TableVisitor _tableVisitor;
        
        private readonly Stack<OperatorToken> _operators;
        private readonly Stack<INode> _operands;
        private readonly Stack<OperatorInvoker> _delayedInvokers;

        public ShuntingYard(TokensTable tokensTable)
        {
            _tokensTable = tokensTable;
            _tableVisitor = new TableVisitor(this);
            
            _operators = new Stack<OperatorToken>();
            _operands = new Stack<INode>();
            _delayedInvokers = new Stack<OperatorInvoker>();
        }

        public void Push(string expression) =>
            VisitTableIteratively(expression.AsSpan());

        public INode Build() =>
            CollapseParsedStack();
        
        private void VisitTableIteratively(ReadOnlySpan<char> expression)
        {
            MatchInfo notParsed = MatchInfo.All(expression);
            
            do notParsed = VisitTableOrThrow(notParsed); 
            while (!notParsed.Empty);
        }

        private INode CollapseParsedStack()
        {
            while(_operators.TryPop(out OperatorToken token))
                InvokeOperator(token.Invoker);
            
            InvokeDelayedOperators();
            
            return _operands.Pop();
        }
        
        private MatchInfo VisitTableOrThrow(MatchInfo notParsed)
        {
            try
            {
                MatchInfo tokenMatch = _tokensTable.Visit(_tableVisitor, notParsed.SliceMatch());
                return notParsed.MoveStart(tokenMatch.End);
            }
            catch (Exception e)
            {
                throw new DiceExpressionParsingException(notParsed.TrimStart().MoveEnd(notParsed.Length - 1), e);
            }
        }

        private void InvokeOperator(OperatorInvoker invoker)
        {
            if (_operands.Count < invoker.RequiredOperands)
                throw new OperatorInvocationException(invoker.RequiredOperands, _operands.Count);
            
            invoker.Invoke(new OperandsStackAccess(this));
        }

        private void InvokeDelayedOperators()
        {
            while(_delayedInvokers.TryPop(out OperatorInvoker invoker))
                InvokeOperator(invoker);
        }

        private void InvokeDelayedOperatorsAfter(OperatorInvoker invoker)
        {
            InvokeOperator(invoker);
            InvokeDelayedOperators();
        }

        private sealed class TableVisitor : TokensTable.IVisitor
        {
            private readonly ShuntingYard _context;
            
            public TableVisitor(ShuntingYard context) 
            {
                _context = context;
            }

            public void OpenParenthesis() =>
                _context._operators.Push(OperatorToken.OpenParenthesis);

            public void CloseParenthesis()
            {
                while (_context._operators.TryPop(out OperatorToken operatorToken))
                {
                    if (operatorToken.IsOpenParenthesis)
                        return;

                    _context.InvokeDelayedOperatorsAfter(operatorToken.Invoker);
                }
            
                throw new UnbalancedParenthesisException();
            }

            public void Operator(int precedence, OperatorInvoker invoker)
            {
                while (_context._operators.TryPeek(out OperatorToken lastOperator) &&
                       !lastOperator.IsOpenParenthesis &&
                       precedence < lastOperator.Precedence)
                    _context.InvokeDelayedOperatorsAfter(_context._operators.Pop().Invoker);

                _context._operators.Push(new OperatorToken(precedence, invoker));
            }

            public void Operand(INumeric operand)
            {
                _context._operands.Push(operand);
                
                _context.InvokeDelayedOperators();
            }

            public void UnknownToken(in MatchInfo tokenMatch) =>
                throw new UnknownTokenException(in tokenMatch);
        }

        public readonly struct OperandsStackAccess
        {
            private readonly ShuntingYard _context;

            public OperandsStackAccess(ShuntingYard context) 
            {
                _context = context;
            }

            public T Pop<T>() where T : INode
            {
                INode node = _context._operands.Pop();
                
                return node is T operand ?
                    operand :
                    throw new OperatorInvocationException(typeof(T), node.GetType());
            }

            public void Push(INode operand) =>
                _context._operands.Push(operand);

            public void ForNextOperand(OperatorInvoker invoker) =>
                _context._delayedInvokers.Push(invoker);
        }
    }
}
