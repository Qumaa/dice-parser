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
            Substring parsed = notParsed;

            try
            {
                ParseSubstring(in notParsed, out parsed);
                return notParsed.MoveStart(parsed.Length).TrimStart();
            }
            catch (Exception e)
            {
                throw _state.MapException(in parsed, e);
            }
        }

        private void ParseSubstring(in Substring notParsed, out Substring parsed)
        {
            if (_state.Tokens.StartsWithOpenParenthesis(in notParsed, out parsed))
            {
                OpenParenthesis(in parsed);
                return;
            }

            if (_state.Tokens.StartsWithCloseParenthesis(in notParsed, out parsed))
            {
                CloseParenthesis();
                return;
            }
            
            if (_state.Tokens.StartsWithOperand(in notParsed, out Operand operand, out parsed))
            {
                Operand(in operand, in parsed);
                return;
            }

            if (_state.Tokens.StartsWithOperator(
                    in notParsed,
                    GetCurrentOperatorUsageForm(),
                    out Operator @operator,
                    out parsed
                    ))
            {
                Operator(in @operator, in parsed);
                return;
            }

            parsed = _state.Tokens.UntilFirstKnownToken(in notParsed, GetCurrentOperatorUsageForm()).Trim();
            throw new UnknownTokenException(in parsed);
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

        private void CloseParenthesis()
        {
            ThrowIfUnbalancedParenthesis();

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
        
        private void ThrowIfUnbalancedParenthesis()
        {
            if (_state.ClosingParenthesisWouldImposeImbalance)
                throw new UnbalancedParenthesisException();
        }
    }
}
