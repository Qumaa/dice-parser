using System;
using System.Text.RegularExpressions;
using DiceRoll.Input;

namespace DiceRoll
{
    public class Program
    {
        // todo: catch exceptions and highlight the spot that caused it
        // todo: interpret string with multiple tokens inside rather than strict one token per string
        // todo: binary/unary operator with same signature ( x - y & -x - -y) 
        public static void Main(string[] args)
        {
            args = new[] { "!", "2", ">", "5"};
            
            RPNExpressionBuilder builder = new(BuildTable());

            foreach (string s in args)
                builder.Push(s);

            var output = builder.Build();
            
            output.Visit(new ProbabilityVisitor());
        }

        private static TokensTable BuildTable()
        {
            TokensTableBuilder builder = new("(", ")");
            
            builder.AddOperandToken(static x => Node.Value.Constant(int.Parse(x)), new Regex(@"^\d+$"));
            builder.AddOperandToken(DiceOperand.Default);
            
            builder.AddOperatorToken<IOperation>(20, static node => Node.Operation.Not(node),"!", "not");
            // builder.AddOperatorToken(200, "-");
            
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
            // builder.AddOperatorToken(60, "&", "&&", "and");
            // builder.AddOperatorToken(60, "|", "||", "or");

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
                    roll => $"Probability of {roll.Outcome.Value} is {ProbabilityToString(roll.Probability)}");

            private static void Print<T>(ProbabilityDistribution<T> distribution, Func<T, string> selector)
            {
                foreach (T value in distribution)
                    Console.WriteLine(selector.Invoke(value));
            }
            
            private static string ProbabilityToString(Probability probability) =>
                $"{probability.Value * 100d:f2}%";
        } 
    }
}
