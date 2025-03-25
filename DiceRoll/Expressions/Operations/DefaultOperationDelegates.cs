using System;
using System.Linq;

namespace DiceRoll.Expressions
{
    public static class DefaultOperationDelegates
    {
        public static OperationDelegate Get(OperationType operationType)
        {
            return operationType switch
            {
                OperationType.Equal => Equal,
                OperationType.NotEqual => Not(Equal),
                OperationType.GreaterThan => GreaterThan,
                OperationType.GreaterThanOrEqual => Not(LessThan),
                OperationType.LessThan => LessThan,
                OperationType.LessThanOrEqual => Not(GreaterThan),
                _ => throw new ArgumentOutOfRangeException(nameof(operationType), operationType, null)
            };
        }

        private static OperationDelegate Not(OperationDelegate operationDelegate) =>
            (left, right) => operationDelegate(left, right).Inversed();

        private static Probability Equal(RollProbabilityDistribution left, RollProbabilityDistribution right)
        {
            if (left.Max.Value < right.Min.Value || right.Max.Value < left.Min.Value)
                return Probability.Zero;
                
            return Evaluate(left, right, (leftRoll, rightRoll) => leftRoll.Value == rightRoll.Value);
        }

        private static Probability GreaterThan(RollProbabilityDistribution left, RollProbabilityDistribution right)
        {
            if (left.Max.Value < right.Min.Value)
                return Probability.Zero;

            return Evaluate(left, right, (leftRoll, rightRoll) => leftRoll.Value > rightRoll.Value);
        }

        private static Probability LessThan(RollProbabilityDistribution left, RollProbabilityDistribution right)
        {
            if ( right.Max.Value < left.Min.Value)
                return Probability.Zero;
                
            return Evaluate(left, right, (leftRoll, rightRoll) => leftRoll.Value < rightRoll.Value);
        }
        
        private static Probability Evaluate(RollProbabilityDistribution left, RollProbabilityDistribution right,
            Func<Outcome, Outcome, bool> predicate) =>
            new(left
                .SelectMany(_ => right, (left, right) => new { left, right })
                .Select(rolls => (rolls, probability: rolls.left.Probability.Value * rolls.right.Probability.Value))
                .Where(t => predicate(t.rolls.left.Outcome, t.rolls.right.Outcome))
                .Select(t => t.probability)
                .Sum());
    }
}
