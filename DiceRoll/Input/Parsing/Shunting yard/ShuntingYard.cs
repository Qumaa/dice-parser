using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DiceRoll.Input
{
    public sealed class ShuntingYard
    {
        private const string _TRAILING_DELAYED_OPERATORS_MESSAGE = "This operator didn't receive enough right-side operands.";

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
                throw new OperatorInvocationException(_TRAILING_DELAYED_OPERATORS_MESSAGE);
            
            while(_operators.TryPop(out OperatorToken token))
                InvokeOperator(token.Invoker);

            // todo
            if (_operands.Count is not 1)
                throw new Exception("trailing operands");
            
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
            int arity = invoker.Arity;
            
            if (_operands.Count < arity)
                throw new OperatorInvocationException(arity, _operands.Count);

            OperandsStackAccess stackAccess = new(this, arity);
            
            invoker.Invoke(in stackAccess);
        }

        private void InvokeDelayedOperators()
        {
            while (_delayedOperators.TryPeek(out DelayedOperatorToken token) &&
                   token.CapturedParenthesisLevel >= _parenthesisLevel &&
                   token.CapturedOperands + token.Invoker.Arity <= _operands.Count)
                InvokeOperator(_delayedOperators.Pop().Invoker);
        }

        private void InvokeAfterDelayedOperators(OperatorInvoker invoker)
        {
            InvokeDelayedOperators();
            InvokeOperator(invoker);
        }

        private void DelayOperatorInvocation(OperatorInvoker invoker) =>
            _delayedOperators.Push(new DelayedOperatorToken(invoker, _parenthesisLevel, _operands.Count));

        // todo: ditch (return to the previous approach with 4 methods on the table class). Will make notParsed always contain the actual info
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

                if (invoker.ExpectsRightOperands)
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

        [StructLayout(LayoutKind.Auto)]
        public readonly struct OperandsStackAccess
        {
            private const string _EXCESSIVE_POPPING_MESSAGE_FORMAT =
                "An attempt to work on more operands than the arity of the operator was intercepted. The operator's arity ({0}) is faulty.";
            private const string _PREMATURE_PUSH_MESSAGE = 
                "An attempt to return the result of working on fewer operands than the declared arity was intercepted.";

            private readonly ShuntingYard _context;
            private readonly int _arity;
            private readonly int _popLimit;

            public OperandsStackAccess(ShuntingYard context, int arity)
            {
                _context = context;
                _arity = arity;
                _popLimit = _context._operands.Count - arity;
            }

            public T Pop<T>() where T : INode
            {
                if (_context._operands.Count - 1 < _popLimit)
                    throw new OperatorInvocationException(ExcessivePoppingMessage());
                
                INode node = _context._operands.Pop();
                
                return node is T operand ?
                    operand :
                    throw new OperatorInvocationException(typeof(T), node.GetType());
            }

            private string ExcessivePoppingMessage() =>
                string.Format(_EXCESSIVE_POPPING_MESSAGE_FORMAT, _arity.ToString());

            public void Push(INode operand)
            {
                if (_context._operands.Count != _popLimit)
                    throw new OperatorInvocationException(_PREMATURE_PUSH_MESSAGE);
                
                _context._operands.Push(operand);
            }
        }

        public abstract class OperatorInvoker
        {
            // positive = straight
            // negative = takes parameters from the right; converted to positive by negating and adding 1
            private readonly int _arity;

            public bool ExpectsRightOperands => _arity <= 0;

            public int Arity => ExpectsRightOperands ? DecodeArityFromRightFlowDirection() : _arity;

            protected OperatorInvoker(int arity, FlowDirection flowDirection = FlowDirection.Left)
            {
                _arity = flowDirection is FlowDirection.Left ?
                    arity :
                    EncodeRightFlowDirectionArity(arity);
            }

            public abstract void Invoke(in OperandsStackAccess operands);

            private int DecodeArityFromRightFlowDirection() =>
                -_arity + 1;

            private static int EncodeRightFlowDirectionArity(int arity) =>
                -(arity - 1);

            protected enum FlowDirection
            {
                // ... 4 3 1 x 2
                Left = 0,
                // x 1 2 3 ...
                Right = 1
                // are (... 3 2 1 x) and (... 4 3 2 x 1) needed?
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
