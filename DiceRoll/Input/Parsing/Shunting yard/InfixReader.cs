using System;

namespace DiceRoll.Input.Parsing
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
            while (!notParsed.IsEmpty);
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
                throw _state.MapException(in output, e);
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
            
            if (_state.Tokens.StartsWithOperand(in notParsed, out Operand operand, out output))
            {
                Operand(in operand, in output);
                return;
            }

            if (_state.Tokens.StartsWithOperator(
                    in notParsed,
                    GetCurrentOperatorUsageForm(),
                    out Operator @operator,
                    out output
                    ))
            {
                Operator(in @operator, in output);
                return;
            }

            output = _state.Tokens.UntilFirstKnownToken(in notParsed, GetCurrentOperatorUsageForm()).Trim();
            throw new UnknownTokenException(in output);
        }

        private OperatorUsageForm GetCurrentOperatorUsageForm() =>
            _state.PrecedingTokenKind is TokenKind.Operand ?
                OperatorUsageForm.Infix :
                OperatorUsageForm.Prefix;

        private void OpenParenthesis(in Substring substring)
        {
            _state.Annotate().ParenthesisOpening();
            
            _operators.Push(in Parsing.Operator.OpenParenthesis, in substring);
        }

        private void CloseParenthesis(in Substring substring)
        {
            ThrowIfUnbalancedParenthesis(in substring);

            while (_operators.TryPop(out Mapped<Operator> operatorToken))
            {
                if (operatorToken.Value.IsOpenParenthesis)
                    break;

                _operators.InvokeOperator(in operatorToken);
            }
            
            _state.Annotate().ParenthesisClosing();
            
            _operators.TryInvokeDelayedOperators();
        }

        private void Operator(in Operator @operator, in Substring operatorSubstring)
        {
            while (_operators.TryPeek(out Operator lastOperator) &&
                   !lastOperator.IsOpenParenthesis &&
                   @operator.Precedence < lastOperator.Precedence)
                _operators.InvokeAfterDelayedOperators(_operators.Pop());

            _operators.Push(in @operator, in operatorSubstring);

            _state.Annotate().OperatorProcessing();
        }

        private void Operand(in Operand operand, in Substring context)
        {
            _operands.Push(in operand, in context);
                
            _state.Annotate().OperandProcessing();
            
            _operators.TryInvokeDelayedOperators();
        }
        
        private void ThrowIfUnbalancedParenthesis(in Substring parenthesisToken)
        {
            if (_state.ClosingParenthesisWouldImposeImbalance)
                _state.MapAndThrow(in parenthesisToken, ParsingErrorMessages.UNBALANCED_PARENTHESIS);
        }
    }
}
