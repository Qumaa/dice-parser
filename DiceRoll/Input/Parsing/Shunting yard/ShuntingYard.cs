using System;

namespace DiceRoll.Input
{
    public sealed class ShuntingYard
    {
        private readonly TokensTable _tokensTable;
        
        private readonly FormulaAccumulator _formulaAccumulator;
        private readonly FormulaTokensStack<OperatorToken> _operators;
        private readonly FormulaTokensStack<INode> _operands;
        private readonly FormulaTokensStack<DelayedOperatorToken> _delayedOperators;

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

        public void Append(string expression)
        {
            _formulaAccumulator.Append(expression);
            ParseTokensIteratively(expression);
        }

        public INode Parse()
        {
            INode node = CollapseOperatorsStack();
            _formulaAccumulator.Clear();
            return node;
        }

        private void ParseTokensIteratively(string expression)
        {
            Substring notParsed = Substring.All(expression).Trim();
            
            do notParsed = ParseSubstringStartOrThrow(in notParsed); 
            while (!notParsed.Empty);
        }

        private INode CollapseOperatorsStack()
        {
            ThrowIfAnyTrailingOperators();

            while(_operators.TryPop(out FormulaToken<OperatorToken> context))
                    InvokeOperatorOrThrow(in context);

            INode result = _operands.PopValue();
            
            ThrowIfAnyOperandLeft();

            return result;
        }

        private Substring ParseSubstringStartOrThrow(in Substring notParsed)
        {
            Substring output = notParsed;

            try
            {
                ParseSubstring(in notParsed, out output);
                return notParsed.MoveStart(output.Length).TrimStart();
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw Wrap(in output, e);
            }
        }

        private void ParseSubstring(in Substring notParsed, out Substring output)
        {
            if (_tokensTable.StartsWithOpenParenthesis(in notParsed, out output))
            {
                OpenParenthesis(in output);
                return;
            }

            if (_tokensTable.StartsWithCloseParenthesis(in notParsed, out output))
            {
                CloseParenthesis(in output);
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

        private void CloseParenthesis(in Substring token)
        {
            ThrowIfUnbalancedParenthesis(in token);

            while (_operators.TryPop(out FormulaToken<OperatorToken> operatorToken))
            {
                if (operatorToken.Value.IsOpenParenthesis)
                {
                    DenoteParenthesisClosing();
                    InvokeDelayedOperators();
                    return;
                }

                InvokeOperatorOrThrow(in operatorToken);
            }
        }

        private void Operator(int precedence, OperatorInvoker invoker, in Substring context)
        {
            while (_operators.TryPeek(out OperatorToken lastOperator) &&
                   !lastOperator.IsOpenParenthesis &&
                   precedence < lastOperator.Precedence)
                InvokeAfterDelayedOperators(_operators.Pop());

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

        private void InvokeOperatorOrThrow(in FormulaToken<OperatorToken> operatorToken)
        {
            try
            {
                InvokeOperator(operatorToken.Value.Invoker);
            }
            catch (Exception e)
            {
                throw Wrap(in operatorToken, e);
            }
        }
        
        private void InvokeOperatorOrThrow(in FormulaToken<DelayedOperatorToken> operatorToken)
        {
            try
            {
                InvokeOperator(operatorToken.Value.Invoker);
            }
            catch (Exception e)
            {
                throw Wrap(in operatorToken, e);
            }
        }

        private void InvokeOperator(OperatorInvoker invoker)
        {
            int arity = invoker.Arity;
            
            if (_operands.Count < arity)
                throw new OperatorInvocationException(ParsingErrorMessages.OperandsExpected(arity, _operands.Count));

            OperandsStackAccess stackAccess = new(_operands, arity);
            
            invoker.Invoke(stackAccess);
        }

        private void InvokeDelayedOperators()
        {
            while (_delayedOperators.TryPeek(out DelayedOperatorToken token) &&
                   token.CapturedParenthesisLevel >= _parenthesisLevel &&
                   token.CapturedOperands + token.Invoker.Arity <= _operands.Count)
                InvokeOperatorOrThrow(_delayedOperators.Pop());
        }

        private void InvokeAfterDelayedOperators(in FormulaToken<OperatorToken> invoker)
        {
            InvokeDelayedOperators();
            InvokeOperatorOrThrow(in invoker);
        }

        private void DelayOperatorInvocation(OperatorInvoker invoker, in Substring context) =>
            _delayedOperators.Push(new DelayedOperatorToken(invoker, _parenthesisLevel, _operands.Count), in context);

        private void ThrowIfUnbalancedParenthesis(in Substring parenthesisToken)
        {
            if (ClosingParenthesisWouldImposeImbalance)
                Throw(in parenthesisToken, ParsingErrorMessages.UNBALANCED_PARENTHESIS);
        }

        private void ThrowIfAnyTrailingOperators()
        {
            if (_delayedOperators.TryPeek(out FormulaToken<DelayedOperatorToken> operatorToken))
                Throw(in operatorToken, ParsingErrorMessages.TRAILING_DELAYED_OPERATOR);
        }
        
        private void ThrowIfAnyOperandLeft()
        {
            if (_operands.TryPeek(out FormulaToken<INode> operandToken))
                Throw(in operandToken, ParsingErrorMessages.UNUSED_OPERAND);
        }

        private void Throw<T>(in FormulaToken<T> context, string message) =>
            throw new ParsingException(_formulaAccumulator.GetFormulaSubstring(in context), message);
        
        private void Throw(in Substring context, string message) =>
            throw new ParsingException(_formulaAccumulator.AppendAndGetSubstring(in context), message);

        private ParsingException Wrap<T>(in FormulaToken<T> context, Exception innerException) =>
            new(_formulaAccumulator.GetFormulaSubstring(in context), innerException);
        
        private ParsingException Wrap(in Substring context, Exception innerException) =>
            new(_formulaAccumulator.AppendAndGetSubstring(in context), innerException);
    }
}
