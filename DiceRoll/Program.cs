using System;

namespace DiceRoll
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var left = Node.Value.Dice(6);
            left = Node.Value.Summation(left, left);
            var right = Node.Value.Dice(6);

            const OperationType operation_type = OperationType.Equal;

            var dist = new DefaultOperation(left, right, operation_type).GetProbabilityDistribution();

            foreach (OptionalRoll roll in dist)
            {
                Console.Write("Probability of ");
                Console.Write(roll.Outcome.ToString());
                Console.Write(" is ");
                Console.WriteLine(roll.Probability.ToString());
            }

            var dist2 = dist.AsLogicalProbabilityDistribution();
            
            foreach (Logical logical in dist2)
            {
                Console.Write("Probability of ");
                Console.Write(logical.Outcome.ToString());
                Console.Write(" is ");
                Console.WriteLine(logical.Probability.ToString());
            }
        }
    }
}
