using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public class TokensTable
    {
        public static readonly TokensTable Default = BuildDefault().Build();

        public readonly IToken OpenParenthesis;

        public readonly IToken CloseParenthesis;

        public readonly OperatorDefinition[] Operators;

        public readonly OperandDefinition[] Operands;

        internal TokensTable(IToken openParenthesis, IToken closeParenthesis, IEnumerable<OperatorDefinition> operators,
            IEnumerable<OperandDefinition> operands)
        {
            OpenParenthesis = openParenthesis;
            CloseParenthesis = closeParenthesis;


            Operators = Syntax.ToArray(operators);
            Operands = Syntax.ToArray(operands);
        }

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
