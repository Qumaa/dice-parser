using System;
using System.Text.RegularExpressions;
using DiceRoll.Input;

namespace DiceRoll
{
    public class Program
    {
        public static void Main(string[] args)
        {
            args = "(1 - 1) 2 / 2".Split(' ');
            
            ExpressionParser parser = new(BuildTable());

            INode output = parser.Parse(args);
            
            output.Visit(new EvaluationVisitor());
        }

        private static TokensTable BuildTable()
        {
            TokensTableBuilder builder = new("(", ")");
            
            builder.AddOperandToken(DiceOperand.Default);
            builder.AddOperandToken(x => Node.Value.Constant(int.Parse(x.AsSpan())), new Regex(@"-?\d+"));
            
            builder.AddOperatorToken<INumeric, INumeric>(100, static (left, right) => Node.Operator.Multiply(left, right), "*", "x");
            builder.AddOperatorToken<INumeric, INumeric>(100, static (left, right) => Node.Operator.DivideRoundUp(left, right), "//");
            builder.AddOperatorToken<INumeric, INumeric>(100, static (left, right) => Node.Operator.DivideRoundDown(left, right), "/");
            
            builder.AddOperatorToken<INumeric, INumeric>(90, static (left, right) => Node.Operator.Add(left, right), "+");
            builder.AddOperatorToken<INumeric, INumeric>(90, static (left, right) => Node.Operator.Subtract(left, right), "-");
            
            builder.AddOperatorToken<INumeric, INumeric>(80, static (left, right) => Node.Operator.GreaterThanOrEqual(left, right), ">=");
            builder.AddOperatorToken<INumeric, INumeric>(80, static (left, right) => Node.Operator.LessThanOrEqual(left, right), "<=");
            builder.AddOperatorToken<INumeric, INumeric>(80, static (left, right) => Node.Operator.GreaterThan(left, right), ">");
            builder.AddOperatorToken<INumeric, INumeric>(80, static (left, right) => Node.Operator.LessThan(left, right), "<");
            
            builder.AddOperatorToken<INumeric, INumeric>(70, static (left, right) => Node.Operator.Equal(left, right), "==", "=", "is");
            builder.AddOperatorToken<INumeric, INumeric>(70, static (left, right) => Node.Operator.NotEqual(left, right), "!=", "=/=");
            
            builder.AddOperatorToken<IAssertion, IAssertion>(60, static (left, right) => Node.Operator.And(left, right), "&&", "&", "and");
            builder.AddOperatorToken<IAssertion, IAssertion>(60, static (left, right) => Node.Operator.Or(left, right), "||", "|", "or");
            
            builder.AddOperatorToken<IAssertion, INumeric>(50, static (left, right) => Node.Value.Conditional(right, left), "?");
            
            builder.AddOperatorToken<IAssertion>(110, static node => Node.Operator.Not(node), "!", "not");
            builder.AddOperatorToken<INumeric>(110, static node => Node.Operator.Negate(node), "-");

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

            public void ForOperation(IOperation operation)
            {
                OptionalRollProbabilityDistribution distribution = operation.GetProbabilityDistribution();

                int rolls = 0;
                foreach (OptionalRoll roll in distribution)
                {
                    Console.Write("Probability of ");

                    if (roll.Outcome.Exists(out Outcome outcome))
                    {
                        Console.Write("rolling ");
                        Console.Write(outcome.Value);
                        rolls++;
                    }
                    else
                        Console.Write("failing");
                    
                    Console.Write(" is ");

                    Console.WriteLine(roll.Probability.ToString());
                }
                
                if (rolls <= 1)
                    return;
                
                Console.Write("Cumulative probability of succeeding is ");
                Console.WriteLine(distribution.False.Inversed().ToString());
            }

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
