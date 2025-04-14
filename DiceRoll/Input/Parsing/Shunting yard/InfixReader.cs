using System;

namespace DiceRoll.Input
{
    internal sealed class InfixReader
    {
        private readonly ShuntingYardState _state;
        private readonly ShuntingYardOperators _operators;
        private readonly ShuntingYardOperands _operands;
        
        public InfixReader(ShuntingYardState state, ShuntingYardOperators operators, ShuntingYardOperands operands)
        {
            _state = state;
            _operators = operators;
            _operands = operands;
        }
        
        public void Read(string expression)
        {
            _state.Mapper.Append(expression);
            ParseTokensIteratively(expression);
        }

        private void ParseTokensIteratively(string expression)
        {
            Substring notParsed = Substring.All(expression).Trim();
            
            do notParsed = ParseSubstringStartOrThrow(in notParsed); 
            while (!notParsed.Empty);
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
                throw _state.Wrap(in output, e);
            }
        }

        private void ParseSubstring(in Substring notParsed, out Substring output)
        {
            if (_state.Tokens.StartsWithOpenParenthesis(in notParsed, out output))
            {
                OpenParenthesis(in output);
                return;
            }

            if (_state.Tokens.StartsWithCloseParenthesis(in notParsed, out output))
            {
                CloseParenthesis(in output);
                return;
            }
            
            if (_state.Tokens.StartsWithOperand(in notParsed, out output, out INumeric operand))
            {
                Operand(operand, in output);
                return;
            }

            bool isUnary = _state.PrecedingTokenKind is TokenKind.ExpressionStart or TokenKind.Operator;
            OperatorKind operatorKind = isUnary ? OperatorKind.Unary : OperatorKind.Binary;

            if (_state.Tokens.StartsWithOperator(
                    in notParsed,
                    operatorKind,
                    out output,
                    out int precedence,
                    out OperatorInvoker invoker
                    ))
            {
                Operator(precedence, invoker, in output);
                return;
            }

            output = _state.Tokens.UntilFirstKnownToken(in notParsed, operatorKind.Reversed()).Trim();
            throw new UnknownTokenException(in output);
        }
        
        private void OpenParenthesis(in Substring context)
        {
            _state.DenoteParenthesisOpening();
            _state.DenoteNewExpressionStart();
            
            _operators.Push(OperatorToken.OpenParenthesis, in context);
        }

        private void CloseParenthesis(in Substring token)
        {
            ThrowIfUnbalancedParenthesis(in token);

            while (_operators.TryPop(out Mapped<OperatorToken> operatorToken))
            {
                if (operatorToken.Value.IsOpenParenthesis)
                    break;

                _operators.InvokeOperatorOrThrow(in operatorToken);
            }
            
            _state.DenoteParenthesisClosing();
            _state.DenoteOperandProcessing();
            
            _operators.InvokeDelayedOperators();
        }

        private void Operator(int precedence, OperatorInvoker invoker, in Substring context)
        {
            while (_operators.TryPeek(out OperatorToken lastOperator) &&
                   !lastOperator.IsOpenParenthesis &&
                   precedence < lastOperator.Precedence)
                _operators.InvokeAfterDelayedOperators(_operators.Pop());

            if (invoker.ExpectsRightOperands)
                _operators.DelayOperatorInvocation(invoker, in context);
            else
                _operators.Push(new OperatorToken(precedence, invoker), in context);
            
            _state.DenoteOperatorProcessing();
        }

        private void Operand(INumeric operand, in Substring context)
        {
            _operands.Push(operand, in context);
                
            _state.DenoteOperandProcessing();
            
            _operators.InvokeDelayedOperators();
        }
        
        private void ThrowIfUnbalancedParenthesis(in Substring parenthesisToken)
        {
            if (_state.ClosingParenthesisWouldImposeImbalance)
                _state.Throw(in parenthesisToken, ParsingErrorMessages.UNBALANCED_PARENTHESIS);
        }
    }
}
