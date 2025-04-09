using System;

namespace DiceRoll
{
    public class Program
    {
        public static void Main(string[] args)
        {
            INumeric left = Node.Value.Dice(6);
            left = Node.Transformation.Multiply(left, Node.Value.Constant(5));
            INumeric right = Node.Value.Dice(6);

            IOperation operation = Node.Operator.GreaterThan(left, right);

            foreach (OptionalRoll roll in operation.GetProbabilityDistribution())
            {
                Console.Write("Probability of ");
                Console.Write(roll.Outcome.ToString());
                Console.Write(" is ");
                Console.WriteLine(roll.Probability.ToString());
            }
        }
    }
}
