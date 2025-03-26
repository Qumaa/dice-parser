using System.Linq;
using DiceRoll.Exceptions;

namespace DiceRoll.Expressions
{
    public static class DefaultOperationDelegates
    {
        public static OperationDelegate Get(OperationType operationType)
        {
            EnumNotDefinedException.ThrowIfNotDefined(operationType);
            
            return operationType switch
            {
                OperationType.Equal => Equal,
                OperationType.NotEqual => Not(Equal),
                OperationType.GreaterThan => GreaterThan,
                OperationType.GreaterThanOrEqual => Not(LessThan),
                OperationType.LessThan => LessThan,
                OperationType.LessThanOrEqual => Not(GreaterThan)
            };
        }

        private static OperationDelegate Not(OperationDelegate operationDelegate) =>
            (left, right) => operationDelegate(left, right).Inversed();

        private static Probability Equal(RollProbabilityDistribution left, RollProbabilityDistribution right)
        {
            if (left.Max < right.Min || left.Min > right.Max)
                return Probability.Zero;

            CDFTable leftTable = new(left);
            CDFTable rightTable = new(right);

            return left.Intersection(right)
                .Aggregate(Probability.Zero, (accumulated, outcome) =>
                    accumulated + (leftTable.EqualTo(outcome) * rightTable.EqualTo(outcome)));
        }

        private static Probability GreaterThan(RollProbabilityDistribution left, RollProbabilityDistribution right)
        {
            if (left.Max <= right.Min)
                return Probability.Zero;

            CDFTable leftTable = new(left);
            CDFTable rightTable = new(right);

            return left.Select(x => x.Outcome)
                .Aggregate(Probability.Zero, 
                    (accumulated, outcome) =>
                        accumulated + (leftTable.EqualTo(outcome) * rightTable.GreaterThan(outcome)));
        }

        private static Probability LessThan(RollProbabilityDistribution left, RollProbabilityDistribution right)
        {
            if ( left.Min >= right.Max)
                return Probability.Zero;
            
            CDFTable leftTable = new(left);
            CDFTable rightTable = new(right);

            return left.Select(x => x.Outcome)
                .Aggregate(Probability.Zero,
                    (accumulated, outcome) =>
                        accumulated + (leftTable.EqualTo(outcome) * rightTable.LessThan(outcome)));
        }
    }
}
