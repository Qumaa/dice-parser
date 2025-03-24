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
            
            Constant modifier = Expression.Constant(1);
            Dice dice = Expression.Dice(6);
            
            ProbabilityDistribution distribution = dice.GetProbabilityDistribution(); // 1d6
            Console.WriteLine(ProbabilityToString(distribution));
            
            distribution = distribution.Combine(distribution); // 2d6
            Console.WriteLine(ProbabilityToString(distribution));
            
            distribution = distribution.Combine(modifier.GetProbabilityDistribution()); // 2d6+1
            Console.WriteLine(ProbabilityToString(distribution));
            
            stopwatch.Stop();
            
            
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


