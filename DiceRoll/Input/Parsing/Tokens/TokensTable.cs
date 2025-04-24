using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiceRoll.Input.Parsing
{
    public class TokensTable
    {
        public static readonly TokensTable Default = BuildDefaultTable();

        private readonly IToken _openParenthesis;

        private readonly IToken _closeParenthesis;

        private readonly Operator[] _operators;

        private readonly Operand[] _operands;

        public TokensTable(IToken openParenthesis, IToken closeParenthesis, IEnumerable<Operator> operators,
            IEnumerable<Operand> operands)
        {
            _openParenthesis = openParenthesis;
            _closeParenthesis = closeParenthesis;
            
            _operators = operators.ToArray();
            _operands = operands.ToArray();
        }

        public bool StartsWithOpenParenthesis(in Substring expression, out Substring tokenMatch) =>
            _openParenthesis.MatchesStart(in expression, out tokenMatch);

        public bool StartsWithCloseParenthesis(in Substring expression, out Substring tokenMatch) =>
            _closeParenthesis.MatchesStart(in expression, out tokenMatch);

        public bool StartsWithOperator(in Substring expression, OperatorArity arity, out Substring tokenMatch, 
            out int precedence, out OperatorInvoker invoker)
        {
            int arityInt = ArityToInt(arity);
            foreach (Operator tokenizedOperator in _operators)
            {
                if (tokenizedOperator.Invoker.Arity != arityInt ||
                    !tokenizedOperator.Token.MatchesStart(in expression, out tokenMatch))
                    continue;

                precedence = tokenizedOperator.Precedence;
                invoker = tokenizedOperator.Invoker;
                return true;
            }

            precedence = 0;
            invoker = null;
            tokenMatch = default;
            return false;
        }

        public bool StartsWithOperand(in Substring expression, out Substring tokenMatch, out INumeric operand)
        {
            foreach (Operand operandToken in _operands)
            {
                if (!operandToken.Token.MatchesStart(in expression, out tokenMatch))
                    continue;

                operand = operandToken.Parse(tokenMatch);
                return true;
            }

            operand = null;
            tokenMatch = default;
            return false;
        }

        public Substring UntilFirstKnownToken(in Substring expression, OperatorArity ignoreOperatorArity)
        {
            return _MatchesAnyToken(in expression, ArityToInt(ignoreOperatorArity), out Substring match) ?
                expression.SetLength(match.Start - expression.Start) :
                expression;

            bool _MatchesAnyToken(in Substring expression, int ignoreArity, out Substring match)
            {
                if (_openParenthesis.Matches(in expression, out match))
                    return true;
                
                if (_closeParenthesis.Matches(in expression, out match))
                    return true;

                foreach (Operator tokenizedOperator in _operators)
                    if (tokenizedOperator.Invoker.Arity != ignoreArity && tokenizedOperator.Token.Matches(in expression, out match))
                        return true;
                
                foreach (Operand tokenizedOperand in _operands)
                    if (tokenizedOperand.Token.Matches(in expression, out match))
                        return true;

                return false;
            }
        }

        private static int ArityToInt(OperatorArity arity) =>
            arity switch
            {
                OperatorArity.Unary => 1,
                OperatorArity.Binary => 2
            };

        private static TokensTable BuildDefaultTable()
        {
            TokensTableBuilder builder = new("(", ")");
            
            builder.AddOperandToken(DiceOperand.Default);
            builder.AddOperandToken(x => Node.Value.Constant(int.Parse(x.AsSpan())), new Regex(@"\d+"));
            
            builder.AddOperatorToken<IAssertion>(110, static node => Node.Operator.Not(node), "!", "not");
            builder.AddOperatorToken<INumeric>(110, static node => Node.Operator.Negate(node), "-");
            
            builder.AddOperatorToken<INumeric, INumeric>(100, static (left, right) => Node.Operator.Multiply(left, right), "*");
            builder.AddOperatorToken<INumeric, INumeric>(100, static (left, right) => Node.Operator.DivideRoundUp(left, right), "//");
            builder.AddOperatorToken<INumeric, INumeric>(100, static (left, right) => Node.Operator.DivideRoundDown(left, right), "/");
            
            builder.AddOperatorToken<INumeric, INumeric>(90, static (left, right) => Node.Operator.Add(left, right), "+");
            builder.AddOperatorToken<INumeric, INumeric>(90, static (left, right) => Node.Operator.Subtract(left, right), "-");
            
            builder.AddOperatorToken<INumeric, INumeric>(80, static (left, right) => Node.Operator.GreaterThanOrEqual(left, right), ">=");
            builder.AddOperatorToken<INumeric, INumeric>(80, static (left, right) => Node.Operator.LessThanOrEqual(left, right), "<=");
            builder.AddOperatorToken<INumeric, INumeric>(80, static (left, right) => Node.Operator.GreaterThan(left, right), ">");
            builder.AddOperatorToken<INumeric, INumeric>(80, static (left, right) => Node.Operator.LessThan(left, right), "<");
            
            builder.AddOperatorToken<INumeric, INumeric>(70, static (left, right) => Node.Operator.Equal(left, right), "==", "=");
            builder.AddOperatorToken<INumeric, INumeric>(70, static (left, right) => Node.Operator.NotEqual(left, right), "!=", "=/=");
            
            builder.AddOperatorToken<IAssertion, IAssertion>(60, static (left, right) => Node.Operator.And(left, right), "&&", "&", "and");
            builder.AddOperatorToken<IAssertion, IAssertion>(60, static (left, right) => Node.Operator.Or(left, right), "||", "|", "or");

            return builder.Build();
        }
    }
}
