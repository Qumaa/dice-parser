using System;
using System.Text.RegularExpressions;
using DiceRoll.Input;

namespace DiceRoll
{
    public class Program
    {
        // todo: binary/unary operator with same signature ( x - y & -x - -y) 
        public static void Main(string[] args)
        {
            args = new[] { "1d20 > 10 ? 1d6"};
            
            DiceExpressionParser parser = new(BuildTable());

            INode output = parser.Parse(string.Concat(args));
            
            output.Visit(new ProbabilityVisitor());
        }

        private static TokensTable BuildTable()
        {
            TokensTableBuilder builder = new("(", ")");
            
            builder.AddOperandToken(DiceOperand.Default);
            builder.AddOperandToken(static x => Node.Value.Constant(int.Parse(x)), new Regex(@"-?\d+"));
            
            builder.AddOperatorToken<IAssertion>(110, static node => Node.Operator.Not(node),"!", "not");
            builder.AddOperatorToken<INumeric>(110, static node => Node.Operator.Negate(node), "-");
            
            builder.AddOperatorToken<INumeric, INumeric>(100, static (left, right) => Node.Operator.Multiply(left, right), "*", "x");
            // builder.AddOperatorToken(100, "/");
            
            builder.AddOperatorToken<INumeric, INumeric>(90, static (left, right) => Node.Operator.Add(left, right), "+");
            // builder.AddOperatorToken(90, "-");
            //
            builder.AddOperatorToken<INumeric, INumeric>(80, static (left, right) => Node.Operator.GreaterThan(left, right), ">");
            // builder.AddOperatorToken(80, ">=");
            // builder.AddOperatorToken(80, "<");
            // builder.AddOperatorToken(80, "<=");
            //
            // builder.AddOperatorToken(70, "=", "==");
            // builder.AddOperatorToken(70, "!=", "=/=");
            //
            builder.AddOperatorToken<IAssertion, IAssertion>(60, static (left, right) => Node.Operator.And(left, right), "&", "&&", "and");
            builder.AddOperatorToken<IAssertion, IAssertion>(60, static (left, right) => Node.Operator.Or(left, right), "|", "||", "or");
            
            builder.AddOperatorToken<IAssertion, INumeric>(50, static (left, right) => Node.Value.Conditional(right, left), "?");

            return builder.Build();
        }

        private sealed class EvaluationVisitor : INodeVisitor
        {
            public void ForNumeric(INumeric numeric) =>
                Console.WriteLine(numeric.Evaluate().Value);

            public void ForOperation(IOperation operation) =>
                Console.WriteLine(
                    operation.Evaluate().Exists(out Outcome outcome) ?
                        outcome.ToString() :
                        "Failed to pass."
                    );

            public void ForAssertion(IAssertion assertion) =>
                Console.WriteLine(assertion.Evaluate().ToString());
        } 
        private sealed class ProbabilityVisitor : INodeVisitor
        {
            public void ForNumeric(INumeric numeric) =>
                Print(
                    numeric.GetProbabilityDistribution(),
                    roll => $"Probability of {roll.Outcome.Value} is {roll.Probability}"
                    );

            public void ForOperation(IOperation operation) =>
                Print(
                    operation.GetProbabilityDistribution(),
                    roll => $"Probability of {roll.Outcome} is {roll.Probability}"
                    );

            public void ForAssertion(IAssertion assertion) =>
                Print(
                    assertion.GetProbabilityDistribution(),
                    logical => $"Probability of {logical.Outcome} is {logical.Probability}"
                    );

            private static void Print<T>(ProbabilityDistribution<T> distribution, Func<T, string> selector)
            {
                foreach (T value in distribution)
                    Console.WriteLine(selector.Invoke(value));
            }
        } 
    }
}
