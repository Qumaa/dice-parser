using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DiceRoll.Input
{
    public sealed class ShuntingYard
    {
        private readonly TokensTable _tokensTable;
        private readonly TableVisitor _tableVisitor;
        
        private readonly Stack<OperatorToken> _operators;
        private readonly Stack<INode> _operands;
        private readonly Stack<DelayedOperatorToken> _delayedOperators;

        private int _parenthesisLevel;

        public ShuntingYard(TokensTable tokensTable)
        {
            _tokensTable = tokensTable;
            _tableVisitor = new TableVisitor(this);
            
            _operators = new Stack<OperatorToken>();
            _operands = new Stack<INode>();
            _delayedOperators = new Stack<DelayedOperatorToken>();
            _parenthesisLevel = 0;
        }

        public void Push(string expression) =>
            VisitTableIteratively(expression.AsSpan());

        public INode Build() =>
            CollapseOperatorsStack();
        
        private void VisitTableIteratively(ReadOnlySpan<char> expression)
        {
            MatchInfo notParsed = MatchInfo.All(expression);
            
            do notParsed = VisitTableOrThrow(notParsed); 
            while (!notParsed.Empty);
        }

        private INode CollapseOperatorsStack()
        {
            if (_delayedOperators.Count > 0)
                throw new OperatorInvocationException("One or multiple right associative operators didn't receive their input.");
            
            while(_operators.TryPop(out OperatorToken token))
                InvokeOperator(token.Invoker);
            
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
            int operandsRequired = invoker.RequiredOperands;
            
            if (_operands.Count < operandsRequired)
                throw new OperatorInvocationException(operandsRequired, _operands.Count);
            
            invoker.Invoke(new OperandsStackAccess(this));
        }

        private void InvokeDelayedOperators()
        {
            while (_delayedOperators.TryPeek(out DelayedOperatorToken token) &&
                   token.CapturedParenthesisLevel >= _parenthesisLevel &&
                   token.CapturedOperands + token.Invoker.RequiredOperands <= _operands.Count)
                InvokeOperator(_delayedOperators.Pop().Invoker);
        }

        private void InvokeAfterDelayedOperators(OperatorInvoker invoker)
        {
            InvokeDelayedOperators();
            InvokeOperator(invoker);
        }

        private void DelayOperatorInvocation(OperatorInvoker invoker) =>
            _delayedOperators.Push(new DelayedOperatorToken(invoker, _parenthesisLevel, _operands.Count));

        private sealed class TableVisitor : TokensTable.IVisitor
        {
            private readonly ShuntingYard _context;

            private bool ClosingParenthesisWouldImposeImbalance => _context._parenthesisLevel is 0;

            public TableVisitor(ShuntingYard context) 
            {
                _context = context;
            }

            public void OpenParenthesis()
            {
                DenoteParenthesisOpening();
                _context._operators.Push(OperatorToken.OpenParenthesis);
            }

            public void CloseParenthesis()
            {
                if (ClosingParenthesisWouldImposeImbalance)
                    throw new UnbalancedParenthesisException();
                
                while (_context._operators.TryPop(out OperatorToken operatorToken))
                {
                    if (operatorToken.IsOpenParenthesis)
                    {
                        DenoteParenthesisClosing();
                        _context.InvokeDelayedOperators();
                        return;
                    }

                    _context.InvokeOperator(operatorToken.Invoker);
                }
            }

            public void Operator(int precedence, OperatorInvoker invoker)
            {
                while (_context._operators.TryPeek(out OperatorToken lastOperator) &&
                       !lastOperator.IsOpenParenthesis &&
                       precedence < lastOperator.Precedence)
                    _context.InvokeAfterDelayedOperators(_context._operators.Pop().Invoker);

                if (invoker.IsRightAssociative)
                    _context.DelayOperatorInvocation(invoker);
                else
                    _context._operators.Push(new OperatorToken(precedence, invoker));
            }

            public void Operand(INumeric operand)
            {
                _context._operands.Push(operand);
                
                _context.InvokeDelayedOperators();
            }

            public void UnknownToken(in MatchInfo tokenMatch) =>
                throw new UnknownTokenException(in tokenMatch);

            private void DenoteParenthesisOpening() =>
                _context._parenthesisLevel++;

            private void DenoteParenthesisClosing() =>
                _context._parenthesisLevel--;
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
        }

        public abstract class OperatorInvoker
        {
            private readonly int _operandsRequired;

            public bool IsRightAssociative => _operandsRequired <= 0;

            public int RequiredOperands => IsRightAssociative ? -_operandsRequired + 1 : _operandsRequired;

            protected OperatorInvoker(int operandsRequired, Associativity associativity = Associativity.Left)
            {
                _operandsRequired = associativity is Associativity.Left ?
                    operandsRequired :
                    EncodeAsRightAssociative(operandsRequired);
            }

            public abstract void Invoke(OperandsStackAccess operands);

            private static int EncodeAsRightAssociative(int operandsRequired) =>
                -(operandsRequired - 1);

            protected enum Associativity
            {
                Left = 0,
                Right = 1
            }
        }
        
        [StructLayout(LayoutKind.Auto)]
        private readonly struct OperatorToken
        {
            public readonly int Precedence;
            public readonly OperatorInvoker Invoker;

            public bool IsOpenParenthesis => Invoker is null;
        
            public static OperatorToken OpenParenthesis => new();

            public OperatorToken(int precedence, OperatorInvoker invoker)
            {
                Precedence = precedence;
                Invoker = invoker;
            }
        }

        [StructLayout(LayoutKind.Auto)]
        private readonly struct DelayedOperatorToken
        {
            public readonly OperatorInvoker Invoker;
            public readonly int CapturedParenthesisLevel;
            public readonly int CapturedOperands;
            
            public DelayedOperatorToken(OperatorInvoker invoker, int capturedParenthesisLevel, int capturedOperands)
            {
                Invoker = invoker;
                CapturedParenthesisLevel = capturedParenthesisLevel;
                CapturedOperands = capturedOperands;
            }
        }
    }
}
