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
            IAnalyzable d20 = Node.Value.Dice(20);
            IAnalyzable adv = Node.Value.Highest(d20, d20);

            IAnalyzable threshold = Node.Value.Constant(14);
            
            IOperation op = Node.Operation.GreaterThanOrEqual(adv, threshold);
            
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
        
        private static string OutcomeToString(Outcome outcome) =>
            outcome.Value.ToString();
    }
}


