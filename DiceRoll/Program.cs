using System;
using System.Diagnostics;
using System.Linq;
using DiceRoll.Nodes;

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
            IAnalyzable d6 = Node.Value.Dice(6);
            IAnalyzable adv = Node.Value.Highest(d6, d6);
            IAnalyzable dis = Node.Value.Lowest(d6, d6);
            IAnalyzable sum = Node.Value.Summation(d6, d6);

            Console.WriteLine(ProbabilityToString(adv.GetProbabilityDistribution()));
            Console.WriteLine(ProbabilityToString(dis.GetProbabilityDistribution()));
            Console.WriteLine(ProbabilityToString(sum.GetProbabilityDistribution()));
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


