using System;
using System.Diagnostics;
using System.Linq;
using Dice.Expressions;

namespace Dice
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            Constant dc = Expression.Constant(10);
            Expressions.Dice dice = Expression.Dice(20);
            OperationExpression dcExpr = Expression.GreaterThanOrEqual(dice, dc);
            Probability probability = dcExpr.Evaluate();
            
            stopwatch.Stop();
            
            Console.WriteLine(LikelihoodToString(probability));
            
            Console.WriteLine($"Elapsed {stopwatch.ElapsedMilliseconds}ms ({stopwatch.Elapsed.Seconds:f2}s)");
        }

        private static string ProbabilityToString(ProbabilityDistribution distribution) =>
            distribution.Aggregate("",
                (s, probability) =>
                    s + $"Probability of {probability.Outcome.Value} is {LikelihoodToString(probability.Probability)}%\n");

        private static string LikelihoodToString(Probability probability) =>
            $"{probability.Value * 100d:F2}%";
    }
}


