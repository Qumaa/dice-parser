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
            Dice dice = Expression.Values.Dice(20);
            Constant constant = Expression.Values.Constant(10);

            Operation roll = Expression.Operations.GreaterThan(dice, constant);
            
            Console.WriteLine(roll.Evaluate().Value);
            Console.WriteLine();
            Console.WriteLine(ProbabilityToString(roll.GetProbabilityDistribution()));
        }

        private static string ProbabilityToString(RollProbabilityDistribution distribution) =>
            distribution.Aggregate("",
                (s, roll) =>
                    s + $"Probability of {roll.Outcome.Value} is {ProbabilityToString(roll.Probability)}%\n");
        
        private static string ProbabilityToString(BinaryProbabilityDistribution distribution) =>
            distribution.Aggregate("",
                (s, binary) =>
                    s + $"Probability of {binary.Value} is {ProbabilityToString(binary.Probability)}%\n");

        private static string ProbabilityToString(Probability probability) =>
            $"{probability.Value * 100d:F2}%";
    }
}


