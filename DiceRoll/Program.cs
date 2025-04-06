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
            
            builder.AddOperatorToken<IOperation>(110, static node => Node.Operation.Not(node),"!", "not");
            builder.AddOperatorToken<INumeric>(110, static node => Node.Transformation.Negate(node), "-");
            
            builder.AddOperatorToken<INumeric, INumeric>(100, static (left, right) => Node.Transformation.Multiply(left, right), "*", "x");
            // builder.AddOperatorToken(100, "/");
            
            builder.AddOperatorToken<INumeric, INumeric>(90, static (left, right) => Node.Transformation.Add(left, right), "+");
            // builder.AddOperatorToken(90, "-");
            //
            builder.AddOperatorToken<INumeric, INumeric>(80, static (left, right) => Node.Operation.GreaterThan(left, right), ">");
            // builder.AddOperatorToken(80, ">=");
            // builder.AddOperatorToken(80, "<");
            // builder.AddOperatorToken(80, "<=");
            //
            // builder.AddOperatorToken(70, "=", "==");
            // builder.AddOperatorToken(70, "!=", "=/=");
            //
            builder.AddOperatorToken<IOperation, IOperation>(60, static (left, right) => Node.Operation.And(left, right), "&", "&&", "and");
            builder.AddOperatorToken<IOperation, IOperation>(60, static (left, right) => Node.Operation.Or(left, right), "|", "||", "or");
            
            builder.AddOperatorToken<IOperation, INumeric>(50, static (left, right) => Node.Value.Conditional(right, left), "?");

            return builder.Build();
        }

        private sealed class RollVisitor : INodeVisitor
        {
            public void ForNumeric(INumeric numeric) =>
                Console.WriteLine(numeric.Evaluate().Value);

            public void ForOperation(IOperation operation) =>
                Console.WriteLine(operation.Evaluate().Value);

            public void ForConditional(IConditional conditional) =>
                Console.WriteLine(conditional.Evaluate().Exists(out Outcome outcome) ?
                    outcome.Value.ToString() :
                    "Didn't pass.");
        } 
        private sealed class ProbabilityVisitor : INodeVisitor
        {
            public void ForNumeric(INumeric numeric) =>
                Print(numeric.GetProbabilityDistribution(),
                    roll => $"Probability of {roll.Outcome.Value} is {ProbabilityToString(roll.Probability)}");

            public void ForOperation(IOperation operation) =>
                Print(operation.GetProbabilityDistribution(),
                    logical => $"Probability of {logical.Outcome.Value} is {ProbabilityToString(logical.Probability)}");

            public void ForConditional(IConditional conditional) =>
                Print(conditional.GetProbabilityDistribution(),
                    roll => $"Probability of {OptionalRollToString(roll.Outcome)} is {ProbabilityToString(roll.Probability)}");

            private static void Print<T>(ProbabilityDistribution<T> distribution, Func<T, string> selector)
            {
                foreach (T value in distribution)
                    Console.WriteLine(selector.Invoke(value));
            }
            
            private static string ProbabilityToString(Probability probability) =>
                $"{probability.Value * 100d:f2}%";
            
            private static string OptionalRollToString(Optional<Outcome> optional) =>
            optional.Exists(out Outcome value) ? value.Value.ToString() : "-";
        } 
    }
}
