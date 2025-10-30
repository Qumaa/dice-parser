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

        private void ParseTokensIteratively(string expression) =>
            ParseTokensIteratively(Substring.All(expression));

        private void ParseTokensIteratively(in Substring expression)
        {
            Substring notParsed = expression.Trim();
            
            do notParsed = ParseSubstringStartOrThrow(in notParsed); 
            while (!notParsed.IsEmpty);
        }
        
        private Substring ParseSubstringStartOrThrow(in Substring notParsed)
        {
            Substring parsed = notParsed;

            try
            {
                ParseSubstringStart(in notParsed, out parsed);
                return notParsed.MoveStart(parsed.Length).TrimStart();
            }
            catch (Exception e)
            {
                throw _state.MapException(in parsed, e);
            }
        }

        private void ParseSubstringStart(in Substring notParsed, out Substring parsed)
        {
            if (_state.Tokens.StartsWithOperand(in notParsed, out OperandDefinition operandDefinition, out parsed))
            {
                Operand(operandDefinition, in parsed);
                return;
            }

            if (_state.Tokens.StartsWithOperator(
                    in notParsed,
                    GetCurrentOperatorUsageForm(),
                    out OperatorDefinition operatorDefinition,
                    out parsed
                    ))
            {
                Operator(operatorDefinition, in parsed);
                return;
            }
            
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

            parsed = _state.Tokens.UntilFirstKnownToken(in notParsed, GetCurrentOperatorUsageForm()).Trim();
            throw new UnknownTokenException(in parsed);
        }

        private OperatorUsageForm GetCurrentOperatorUsageForm() =>
            _state.PrecedingTokenKind is TokenKind.Operand ?
                OperatorUsageForm.Infix :
                OperatorUsageForm.Prefix;

        private void OpenParenthesis(in Substring substring)
        {
            _operators.OpenParenthesis(in substring);
            
            _state.Annotate().ParenthesisOpening();
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

        private void Operator(OperatorDefinition definition, in Substring substring)
        {
            while (_operators.TryPeek(out Operator lastOperator) &&
                   !lastOperator.IsOpenParenthesis &&
                   definition.Precedence < lastOperator.Precedence)
                _operators.InvokeAfterDelayedOperators(_operators.Pop());

            Operator @operator = new(definition);
            
            OperatorProcessingResult processingResult = _operators.Process(in @operator, in substring);

            _state.Annotate().OperatorProcessing(processingResult);
        }

        private void Operand(OperandDefinition definition, in Substring substring)
        {
            Operand operand = ParseOperand(definition, in substring);
            
            _operands.Push(in operand, in substring);
                
            _state.Annotate().OperandProcessing();
            
            _operators.TryInvokeDelayedOperators();
        }

        private Operand ParseOperand(OperandDefinition definition, in Substring substring)
        {
            OperandParser parser = definition.Parser;

            INode parsedNode = parser switch
            {
                FlatOperandParser flatParser => flatParser.Parse(in substring),
                RecursiveOperandParser recursiveParser => recursiveParser.Parse(in substring, new InlineParser(this)),
                _ => throw new ArgumentException(
                    $"Unsupported operand parser type. Implementations of either {nameof(FlatOperandParser)} or {nameof(RecursiveOperandParser)} are expected."
                    )
            };

            return new Operand(parsedNode, definition.EvaluationType);
        }

        private void ThrowIfUnbalancedParenthesis()
        {
            if (_state.ClosingParenthesisWouldImposeImbalance)
                throw new UnbalancedParenthesisException();
        }

        // todo this lazy shit patch barely works
        private sealed class InlineParser : FlatOperandParser
        {
            private readonly InfixReader _infixReader;
            private readonly int _capturedOperands;
            
            public InlineParser(InfixReader infixReader)
            {
                _infixReader = infixReader;
                _capturedOperands = infixReader._state.Operands.Count;
            }

            public override INode Parse(in Substring expression)
            {
                _infixReader.OpenParenthesis(default); // todo
                _infixReader.ParseTokensIteratively(in expression);
                _infixReader.CloseParenthesis();
                
                // todo if not 1 operands produced then throw

                return _infixReader._operands.Pop().Value.Node;
            }
        }
    }
}
