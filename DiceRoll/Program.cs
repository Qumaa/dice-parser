using System;

namespace DiceRoll
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var d6 = Node.Value.Dice(6);
            var d20 = Node.Value.Dice(4);

            NumericOperationOptionalRollsDelegate optionalRollsDelegate =
                DefaultNumericOperationDelegates.RollsProbability.Get(NumericOperationType.LessThanOrEqual);

            var dist = optionalRollsDelegate(d6.GetProbabilityDistribution(), d20.GetProbabilityDistribution());

            foreach (OptionalRoll roll in dist)
            {
                Console.Write("Probability of ");
                Console.Write(roll.Outcome.ToString("false"));
                Console.Write(" is ");
                Console.WriteLine(roll.Probability.ToString());
            }
        }
    }
}
