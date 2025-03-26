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
            Dice d20 = Expression.Value.Dice(20);
            Composite adv = Expression.Value.Composite<KeepHighest>(d20, d20);

            Constant dc = Expression.Value.Constant(10);

            Operation op = Expression.Operation.GreaterThanOrEqual(adv, dc);

            Console.WriteLine(ProbabilityToString(op.GetProbabilityDistribution()));
        }

        private static string ProbabilityToString(RollProbabilityDistribution distribution) =>
            distribution.Aggregate("",
                (s, roll) =>
                    s + $"Probability of {roll.Outcome.Value} is {ProbabilityToString(roll.Probability)}%\n");
        
        private static string ProbabilityToString(LogicalProbabilityDistribution distribution) =>
            distribution.Aggregate("",
                (s, binary) =>
                    s + $"Probability of {binary.Outcome.Value} is {ProbabilityToString(binary.Probability)}%\n");

        private static string ProbabilityToString(Probability probability) =>
            $"{probability.Value * 100d:F2}%";
    }
}


