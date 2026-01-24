using System.Collections.Generic;
using System.Linq;
using DiceRoll.FluentExtensions;

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


            Operators = operators as OperatorDefinition[] ?? operators.ToArray();
            Operands = operands as OperandDefinition[] ?? operands.ToArray();
        }

        public static TokensTableBuilder BuildDefault() =>
            new TokensTableBuilder(Token("("), Token(")"))
                .Operand(in NumericOperand.Default)
                .Operand(in BinaryOperand.Default)
                
                .PrefixUnaryOperator(
                    new DieToken(Token("d")),
                    1001,
                    static (INumeric faces) => Node.Value.Die(faces.Evaluate())
                    )
                
                .BinaryOperator(
                    new DiceToken(Token("d")),
                    1000,
                    static (INumeric left, INumeric right) => Node.Value.Dice(right.Evaluate(), left.Evaluate()),
                    Associativity.Right
                    )
                
                // todo min/max composite
                // should be as easy as doing constant with a value of min/max of a probability dist for composite
                // think of non-distributable seq
                // 1..8 should be 8
                // d4:8 should be 4?
                // (1, 2, 3d6) should be max of 3d6 so 18
                // iterate through source nodes and aggregate the value based on their dist?
                
                .PostfixUnaryOperator(
                    Token("s", "sum", "summation", "total"),
                    950,
                    static (ISequence<INumeric> nodes) => Node.Value.Composite<Total>(nodes)
                    )
                .PostfixUnaryOperator(
                    Token("h", "highest"),
                    950,
                    (ISequence<INumeric> nodes) => Node.Value.Composite<KeepHighest>(nodes)
                    )
                // todo infix that specifies number of highest to take (4d6 highest 3)
                .PostfixUnaryOperator(
                    Token("l", "lowest"),
                    950,
                    static (ISequence<INumeric> nodes) => Node.Value.Composite<KeepLowest>(nodes)
                    )
                // todo infix that specifies number of lowest to take (4d6 lowest 3)
                
                .BinaryOperator(
                    Token(":", "times"),
                    900,
                    static (INumeric left, INumeric right) => Node.Value.Sequence(left, right.Evaluate())
                    )
                
                .BinaryOperator(
                    Token("..", "through"),
                    850,
                    static (INumeric left, INumeric right) =>
                    {
                        Outcome lo = left.Evaluate();
                        Outcome ro = right.Evaluate();
                        
                        IEnumerable<int> ints = Enumerable.Range(lo, ro - lo);
                        IEnumerable<INumeric> nodes = ints.Select(x => x.AsConstant());

                        return Node.Value.Sequence(nodes);
                    }
                    )
                
                .PrefixUnaryOperator(Token("!", "not"), 120, static (IAssertion node) => node.Not())
                .PrefixUnaryOperator(Token("-"), 120, static (INumeric node) => node.Negate())
                
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
                .Overload(static (INumeric left, INumeric right) => left.Equal(right))
                .Overload(static (IAssertion left, IAssertion right) => left.Equal(right))
                .Finish()
                
                .OverloadedBinaryOperator(Token("!=", "=/="), 70)
                .Overload(static (INumeric left, INumeric right) => left.NotEqual(right))
                .Overload(static (IAssertion left, IAssertion right) => left.NotEqual(right))
                .Finish()
                
                .BinaryOperator(Token("&&", "&", "and"), 60, static (IAssertion left, IAssertion right) => left.And(right))
                .BinaryOperator(Token("||", "|", "or"), 60, static (IAssertion left, IAssertion right) => left.Or(right));

        private static StringComparisonToken Token(params string[] values) =>
            StringComparisonToken.CaseInsensitive(values);
    }
    
    public static class TokensTableExtensions
    {
        public static LexingPipeline ToDefaultPipeline(this TokensTable table, LexemesList output) =>
            new(
                new Lexer[]
                {
                    new OpenParenthesisLexer(table.OpenParenthesis, output),
                    new CloseParenthesisLexer(table.CloseParenthesis, output),
                    new OperandLexer(table.Operands, output),
                    new OperatorLexer(table.Operators, output)
                }
                );
    }
}
