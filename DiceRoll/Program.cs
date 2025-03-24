using System;
using System.Diagnostics;
using System.Linq;
using DiceRoll.Expressions;

namespace DiceRoll
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            
            Body();

            stopwatch.Stop();
            
            Console.WriteLine($"Elapsed {stopwatch.ElapsedMilliseconds}ms ({stopwatch.Elapsed.Seconds:f2}s)");
        }

        private static void Body()
        {
            Dice dice1 = Expression.Values.Dice(6);
            Dice dice2 = Expression.Values.Dice(10);

            Composite roll = Expression.Values.Composite<KeepLowest>( dice1, dice2);
            
            Console.WriteLine(roll.Evaluate().Value);
            Console.WriteLine();
            Console.WriteLine(ProbabilityToString(roll.GetProbabilityDistribution()));
        }

        private static string ProbabilityToString(ProbabilityDistribution distribution) =>
            distribution.Aggregate("",
                (s, probability) =>
                    s + $"Probability of {probability.Outcome.Value} is {LikelihoodToString(probability.Probability)}%\n");

        private static string LikelihoodToString(Probability probability) =>
            $"{probability.Value * 100d:F2}%";
    }
}


