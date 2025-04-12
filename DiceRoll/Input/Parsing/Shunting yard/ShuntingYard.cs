using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DiceRoll.Input
{
    public sealed class ShuntingYard
    {
        private const string _TRAILING_DELAYED_OPERATORS_MESSAGE = "This operator didn't receive enough right-side operands.";

        private readonly TokensTable _tokensTable;
        
        private readonly FormulaAccumulator _formulaAccumulator;
        private readonly FormulaSubstringsStack<OperatorToken> _operators;
        private readonly FormulaSubstringsStack<INode> _operands;
        private readonly FormulaSubstringsStack<DelayedOperatorToken> _delayedOperators;

        private int _parenthesisLevel;
        
        private bool ClosingParenthesisWouldImposeImbalance => _parenthesisLevel is 0;

        public ShuntingYard(TokensTable tokensTable)
        {
            _tokensTable = tokensTable;

            _formulaAccumulator = new FormulaAccumulator();
            
            _operators = _formulaAccumulator.CreateStack<OperatorToken>();
            _operands = _formulaAccumulator.CreateStack<INode>();
            _delayedOperators = _formulaAccumulator.CreateStack<DelayedOperatorToken>();
            
            _parenthesisLevel = 0;
        }

        public void Push(string expression)
        {
            ParseTokensIteratively(expression);
            _formulaAccumulator.Accumulate(expression);
        }

        public INode Build()
        {
            INode node = CollapseOperatorsStack();
            _formulaAccumulator.Clear();
            return node;
        }

        private void ParseTokensIteratively(string expression)
        {
            Substring notParsed = Substring.All(expression).Trim();
            
            do notParsed = ParseSubstringStart(in notParsed); 
            while (!notParsed.Empty);
        }

        private INode CollapseOperatorsStack()
        {
            ThrowIfAnyTrailingOperators();

            FormulaSubstring<OperatorToken> context = default;

            try
            {
                while(_operators.TryPop(out context))
                    InvokeOperator(context.Value.Invoker);
            }
            catch (FormulaParsingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FormulaParsingException(_formulaAccumulator.AccumulatedFormulaSubstring(in context), e);
            }

            INode result = _operands.PopValue();
            
            ThrowIfAnyOperandLeft();

            return result;
        }

    #region Tokens Parsing

        private Substring ParseSubstringStart(in Substring notParsed)
        {
            Substring output = notParsed;

            try
            {
                ParseSubstringStartOrThrow(in notParsed, out output);
                return notParsed.MoveStart(output.Length).TrimStart();
            }
            catch (FormulaParsingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FormulaParsingException(_formulaAccumulator.AccumulateAndBuild(in output), e);
            }
        }

        private void ParseSubstringStartOrThrow(in Substring notParsed, out Substring output)
        {
            if (_tokensTable.StartsWithOpenParenthesis(in notParsed, out output))
            {
                OpenParenthesis(in output);
                return;
            }

            if (_tokensTable.StartsWithCloseParenthesis(in notParsed, out output))
            {
                CloseParenthesis();
                return;
            }
            
            if (_tokensTable.StartsWithOperand(in notParsed, out output, out INumeric operand))
            {
                Operand(operand, in output);
                return;
            }

            if (_tokensTable.StartsWithOperator(in notParsed, out output, out int precedence, out OperatorInvoker invoker))
            {
                Operator(precedence, invoker, in output);
                return;
            }

            output = _tokensTable.UntilFirstKnownToken(in notParsed).Trim();
            throw new UnknownTokenException(in output);
        }

        private void OpenParenthesis(in Substring context)
        {
            DenoteParenthesisOpening();
            _operators.Push(OperatorToken.OpenParenthesis, in context);
        }

        private void CloseParenthesis()
        {
            if (ClosingParenthesisWouldImposeImbalance)
                throw new UnbalancedParenthesisException();
                
            while (_operators.TryPop(out OperatorToken operatorToken))
            {
                if (operatorToken.IsOpenParenthesis)
                {
                    DenoteParenthesisClosing();
                    InvokeDelayedOperators();
                    return;
                }

                InvokeOperator(operatorToken.Invoker);
            }
        }

        private void Operator(int precedence, OperatorInvoker invoker, in Substring context)
        {
            while (_operators.TryPeek(out OperatorToken lastOperator) &&
                   !lastOperator.IsOpenParenthesis &&
                   precedence < lastOperator.Precedence)
                InvokeAfterDelayedOperators(_operators.PopValue().Invoker);

            if (invoker.ExpectsRightOperands)
                DelayOperatorInvocation(invoker, in context);
            else
                _operators.Push(new OperatorToken(precedence, invoker), in context);
        }

        private void Operand(INumeric operand, in Substring context)
        {
            _operands.Push(operand, in context);
                
            InvokeDelayedOperators();
        }

        private void DenoteParenthesisOpening() =>
            _parenthesisLevel++;

        private void DenoteParenthesisClosing() =>
            _parenthesisLevel--;

    #endregion

    #region Operators Invocation

        private void InvokeOperator(OperatorInvoker invoker)
        {
            int arity = invoker.Arity;
            
            if (_operands.Count < arity)
                throw new OperatorInvocationException(arity, _operands.Count);

            OperandsStackAccess stackAccess = new(this, arity);
            
            invoker.Invoke(stackAccess);
        }

        private void InvokeDelayedOperators()
        {
            while (_delayedOperators.TryPeek(out DelayedOperatorToken token) &&
                   token.CapturedParenthesisLevel >= _parenthesisLevel &&
                   token.CapturedOperands + token.Invoker.Arity <= _operands.Count)
                InvokeOperator(_delayedOperators.Pop().Value.Invoker);
        }

        private void InvokeAfterDelayedOperators(OperatorInvoker invoker)
        {
            InvokeDelayedOperators();
            InvokeOperator(invoker);
        }

        private void DelayOperatorInvocation(OperatorInvoker invoker, in Substring context) =>
            _delayedOperators.Push(new DelayedOperatorToken(invoker, _parenthesisLevel, _operands.Count), in context);

    #endregion

    #region Throws

        private void ThrowIfAnyTrailingOperators()
        {
            if (_delayedOperators.TryPeek(out FormulaSubstring<DelayedOperatorToken> context))
                throw new FormulaParsingException(
                    _formulaAccumulator.AccumulatedFormulaSubstring(in context),
                    _TRAILING_DELAYED_OPERATORS_MESSAGE
                    );
        }
        
        private void ThrowIfAnyOperandLeft()
        {
            if (_operands.TryPeek(out FormulaSubstring<INode> context))
                throw new FormulaParsingException(_formulaAccumulator.AccumulatedFormulaSubstring(in context), "trailing operands");
        }

    #endregion

    #region Nested Types

        private sealed class FormulaAccumulator
        {
            private readonly List<string> _accumulatedFormula = new();
            private int _formulaLength = 0;

            public FormulaSubstring<T> Wrap<T>(in T element, in Substring token) =>
                Wrap(in element, token.Start, token.Length);
            
            public FormulaSubstring<T> Wrap<T>(in T element, int start, int length) =>
                new(in element, _formulaLength + start, length);

            public void Accumulate(string formulaPiece)
            {
                if (_accumulatedFormula.Count > 0 && !char.IsWhiteSpace(_accumulatedFormula[^1][^1]))
                    _accumulatedFormula.Add(" ");
                
                _accumulatedFormula.Add(formulaPiece);
                _formulaLength += formulaPiece.Length + 1;
            }

            public Substring AccumulateAndBuild(in Substring substring)
            {
                int start = substring.Start + _formulaLength;
                
                Range range = new(new Index(start), new Index(start + substring.Length));
                
                Accumulate(substring.Source);

                return AccumulatedFormulaSubstring(range);
            }

            public Substring AccumulatedFormulaSubstring(in Range tokenRange)
            {
                string source = string.Concat(_accumulatedFormula);
                (int Offset, int Length) tuple = tokenRange.GetOffsetAndLength(source.Length);

                return new Substring(source, tuple.Offset, tuple.Length);
            }
            
            public Substring AccumulatedFormulaSubstring<T>(in FormulaSubstring<T> token) =>
                AccumulatedFormulaSubstring(token.Range);

            public void Clear()
            {
                _accumulatedFormula.Clear();
                _formulaLength = 0;
            }

            public FormulaSubstringsStack<T> CreateStack<T>() =>
                new(this);
        }

        private sealed class FormulaSubstringsStack<T>
        {
            private readonly Stack<FormulaSubstring<T>> _stack;
            private readonly FormulaAccumulator _formulaAccumulator;
            
            public FormulaSubstringsStack(FormulaAccumulator formulaAccumulator)
            {
                _formulaAccumulator = formulaAccumulator;

                _stack = new Stack<FormulaSubstring<T>>();
            }

            public int Count => _stack.Count;

            public void Push(in T value, in Substring context) =>
                Push(_formulaAccumulator.Wrap(in value, in context));
            public void Push(in T value, int start, int length) =>
                Push(_formulaAccumulator.Wrap(in value, start, length));
            
            public void Push(in FormulaSubstring<T> context) =>
                _stack.Push(context);
            
            public FormulaSubstring<T> Pop() =>
                _stack.Pop();

            public T PopValue() =>
                Pop().Value;

            public FormulaSubstring<T> Peek() =>
                _stack.Peek();
            public T PeekValue() =>
                Peek().Value;

            public bool TryPeek(out FormulaSubstring<T> context) =>
                _stack.TryPeek(out context);

            public bool TryPeek(out T value)
            {
                if (TryPeek(out FormulaSubstring<T> context))
                {
                    value = context.Value;
                    return true;
                }

                value = default;
                return false;
            }

            public bool TryPop(out FormulaSubstring<T> context) =>
                _stack.TryPop(out context);

            public bool TryPop(out T value)
            {
                if (TryPop(out FormulaSubstring<T> context))
                {
                    value = context.Value;
                    return true;
                }

                value = default;
                return false;
            }
        }

        [StructLayout(LayoutKind.Auto)]
        public struct OperandsStackAccess
        {
            private const string _EXCESSIVE_POPPING_MESSAGE_FORMAT =
                "An attempt to work on more operands than the arity of the operator was intercepted. The operator's arity ({0}) is faulty.";
            private const string _PREMATURE_PUSH_MESSAGE = 
                "An attempt to return the result of working on fewer operands than the declared arity was intercepted.";

            private readonly ShuntingYard _context;
            private readonly int _arity;
            private readonly int _popLimit;

            private int _start;
            private int _length;

            public OperandsStackAccess(ShuntingYard context, int arity)
            {
                _context = context;
                _arity = arity;
                _popLimit = _context._operands.Count - arity;

                FormulaSubstring<INode> peek = _context._operands.Peek();
                _start = peek.Range.Start.Value;
                _length = peek.Range.End.Value - _start;
            }

            public T Pop<T>() where T : INode
            {
                if (_context._operands.Count - 1 < _popLimit)
                    throw new OperatorInvocationException(ExcessivePoppingMessage());

                FormulaSubstring<INode> context = _context._operands.Pop();

                int startDiff = _start - context.Range.Start.Value;
                _start -= startDiff;
                _length += startDiff;
                
                INode node = context.Value;
                
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
                
                _context._operands.Push(operand, _start, _length);
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

            public abstract void Invoke(OperandsStackAccess operands);

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
        
        [StructLayout(LayoutKind.Auto)]
        private readonly struct FormulaSubstring<T>
        {
            public readonly Range Range;
            public readonly T Value;
            
            public FormulaSubstring(in T value, int contextStart, int contextLength)
            {
                Value = value;
                Range = new Range(new Index(contextStart), new Index(contextStart + contextLength));
            }
        }

    #endregion
    }
}
