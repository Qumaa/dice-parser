using System;
using System.Collections.Generic;
using DiceRoll.FluentExtensions;

namespace DiceRoll.Input.Parsing
{
    public class TokensTable
    {
        public static readonly TokensTable Default = BuildDefault().Build();

        private readonly IToken _openParenthesis;

        private readonly IToken _closeParenthesis;

        private readonly OperatorDefinition[] _operators;

        private readonly OperandDefinition[] _operands;

        internal TokensTable(IToken openParenthesis, IToken closeParenthesis, IEnumerable<OperatorDefinition> operators,
            IEnumerable<OperandDefinition> operands)
        {
            _openParenthesis = openParenthesis;
            _closeParenthesis = closeParenthesis;


            _operators = Syntax.ToArray(operators);
            _operands = Syntax.ToArray(operands);
        }

        public bool StartsWithOpenParenthesis(in Substring expression, out Substring substring) =>
            _openParenthesis.MatchesStart(in expression, out substring);

        public bool StartsWithCloseParenthesis(in Substring expression, out Substring substring) =>
            _closeParenthesis.MatchesStart(in expression, out substring);

        public bool StartsWithOperator(in Substring expression, OperatorUsageForm usageForm,
            out OperatorDefinition definition, out Substring substring)
        {
            foreach (OperatorDefinition operatorDefinition in _operators)
            {
                if (!(MatchesUsageForm(operatorDefinition.InvocationBehaviour, usageForm) &&
                      operatorDefinition.Token.MatchesStart(in expression, out substring)))
                    continue;

                definition = operatorDefinition;
                return true;
            }

            substring = default;
            definition = null;
            return false;
        }

        public bool StartsWithOperand(in Substring expression, out OperandDefinition definition,
            out Substring substring)
        {
            foreach (OperandDefinition operandDefinition in _operands)
            {
                if (!operandDefinition.Token.MatchesStart(in expression, out substring))
                    continue;

                definition = operandDefinition;
                return true;
            }

            substring = default;
            definition = null;
            return false;
        }

        public Substring UntilFirstKnownToken(in Substring expression, OperatorUsageForm currentOperatorUsageForm)
        {
            int firstKnownTokenStart = expression.End;
            
            if (_openParenthesis.Matches(in expression, out Substring knownSubstring))
                firstKnownTokenStart = Math.Min(firstKnownTokenStart, knownSubstring.Start);
                
            if (_closeParenthesis.Matches(in expression, out knownSubstring))
                firstKnownTokenStart = Math.Min(firstKnownTokenStart, knownSubstring.Start);

            foreach (OperatorDefinition operatorDefinition in _operators)
                if (MatchesUsageForm(operatorDefinition.InvocationBehaviour, currentOperatorUsageForm) &&
                    operatorDefinition.Token.Matches(in expression, out knownSubstring))
                    firstKnownTokenStart = Math.Min(firstKnownTokenStart, knownSubstring.Start);
                
            foreach (OperandDefinition operandDefinition in _operands)
                if (operandDefinition.Token.Matches(in expression, out knownSubstring))
                    firstKnownTokenStart = Math.Min(firstKnownTokenStart, knownSubstring.Start);

            return expression.SetEnd(firstKnownTokenStart);
        }

        private static bool MatchesUsageForm(OperatorInvocationBehaviour invocationBehaviour, OperatorUsageForm usageForm) =>
            invocationBehaviour is { LeftArity: 0, RightArity: > 0 } == usageForm is OperatorUsageForm.Prefix;

        public static TokensTableBuilder BuildDefault() =>
            new TokensTableBuilder(Token("("), Token(")"))
                .Operand(in DiceOperand.Default)
                .Operand(in NumericOperand.Default)
                .Operand(in BinaryOperand.Default)
                .Operand(in NodePoolOperand.Default)
                
                .PrefixUnaryOperator(Token("!", "not"), 120, static (IAssertion node) => node.Not())
                .PrefixUnaryOperator(Token("-"), 120, static (INumeric node) => node.Negate())
                
                .CompositionOperator(110, in CompositionDefinition.Summation)
                .CompositionOperator(110, in CompositionDefinition.Highest)
                .CompositionOperator(110, in CompositionDefinition.Lowest)
                
                .BinaryOperator(Token("*"), 100, static (INumeric left, INumeric right) => left.Multiply(right))
                .BinaryOperator(Token("//"), 100, static (INumeric left, INumeric right) => left.DivideRoundUp(right))
                .BinaryOperator(Token("/"), 100, static (INumeric left, INumeric right) => left.DivideRoundDown(right))
                
                .BinaryOperator(Token("+"), 90, static (INumeric left, INumeric right) => left.Add(right))
                .BinaryOperator(Token("-"), 90, static (INumeric left, INumeric right) => left.Subtract(right))
                
                .BinaryOperator(Token(">="), 80, static (INumeric left, INumeric right) => left.GreaterThanOrEqual(right))
                .BinaryOperator(Token("<="), 80, static (INumeric left, INumeric right) => left.LessThanOrEqual(right))
                .BinaryOperator(Token(">"), 80, static (INumeric left, INumeric right) => left.GreaterThan(right))
                .BinaryOperator(Token("<"), 80, static (INumeric left, INumeric right) => left.LessThan(right))
                
                .OverloadedBinaryOperator(Token("==", "="), 70)
                    .Overload(OperatorInvoker.Binary(static (INumeric left, INumeric right) => left.Equal(right)))
                    .Overload(OperatorInvoker.Binary(static (IAssertion left, IAssertion right) => left.Equal(right)))
                    .Finish()
                
                .OverloadedBinaryOperator(Token("!=", "=/="), 70)
                    .Overload(OperatorInvoker.Binary(static (INumeric left, INumeric right) => left.NotEqual(right)))
                    .Overload(OperatorInvoker.Binary(static (IAssertion left, IAssertion right) => left.NotEqual(right)))
                    .Finish()
                
                .BinaryOperator(Token("&&", "&", "and"), 60, static (IAssertion left, IAssertion right) => left.And(right))
                .BinaryOperator(Token("||", "|", "or"), 60, static (IAssertion left, IAssertion right) => left.Or(right));

        private static StringComparisonToken Token(params string[] values) =>
            StringComparisonToken.CaseInsensitive(values);
    }
}
