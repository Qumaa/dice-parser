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

            IOperation operation = new DefaultBinaryOperation(left, right, operation_type);

            IAssertion assertion = (IAssertion) operation;
            
            foreach (Logical logical in assertion.GetProbabilityDistribution())
            {
                Console.Write("Probability of ");
                Console.Write(logical.Outcome.ToString());
                Console.Write(" is ");
                Console.WriteLine(logical.Probability.ToString());
            }
        }
    }
}
