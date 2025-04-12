using System;

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

            OperandsStackAccess stackAccess = new(_operands, arity);
            
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
    }
}
