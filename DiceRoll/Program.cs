using System;
using DiceRoll.Input.Parsing;

namespace DiceRoll
{
    public class Program
    {
        public static void Main(string[] args)
        {
            args = "6 + -4 is aa".Split(' ');
            
            ExpressionParser parser = new(TokensTable.Default);

            INode output = parser.Parse(args);
            
            output.Visit(new EvaluationVisitor());
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
