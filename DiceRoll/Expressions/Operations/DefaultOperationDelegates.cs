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

            CDFTable leftTable = new(left);
            CDFTable rightTable = new(right);

            return new Probability(left.Intersection(right)
                .Aggregate(0d, (accumulated, outcome) =>
                        accumulated + (leftTable.EqualTo(outcome).Value * rightTable.EqualTo(outcome).Value)));
        }

        private static Probability GreaterThan(RollProbabilityDistribution left, RollProbabilityDistribution right)
        {
            if (left.Max.Value < right.Min.Value)
                return Probability.Zero;

            CDFTable leftTable = new(left);
            CDFTable rightTable = new(right);

            return new Probability(left.Select(x => x.Outcome)
                .Aggregate(0d,
                    (accumulated, outcome) =>
                        accumulated + (leftTable.EqualTo(outcome).Value * rightTable.GreaterThan(outcome).Value)));
        }

        private static Probability LessThan(RollProbabilityDistribution left, RollProbabilityDistribution right)
        {
            if ( left.Min.Value > right.Max.Value)
                return Probability.Zero;
            
            CDFTable leftTable = new(left);
            CDFTable rightTable = new(right);

            return new Probability(left.Select(x => x.Outcome)
                .Aggregate(0d,
                    (accumulated, outcome) =>
                        accumulated + (leftTable.EqualTo(outcome).Value * rightTable.LessThan(outcome).Value)));
        }
    }
}
