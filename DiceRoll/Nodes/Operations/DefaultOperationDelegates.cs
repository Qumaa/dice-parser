using System.Linq;
using DiceRoll.Exceptions;

namespace DiceRoll.Nodes
{
    public static class DefaultOperationDelegates
    {
        public static OperationDelegates Get(OperationType operationType) =>
            new(Evaluation.Get(operationType), ProbabilityEvaluation.Get(operationType));
        
        public static class Evaluation
        {
            public static OperationDelegate Get(OperationType operationType)
            {
                EnumValueNotDefinedException.ThrowIfValueNotDefined(operationType);

                return operationType switch
                {
                    OperationType.Equal => Equal,
                    OperationType.NotEqual => NotEqual,
                    OperationType.GreaterThan => GreaterThan,
                    OperationType.GreaterThanOrEqual => GreaterThanOrEqual,
                    OperationType.LessThan => LessThan,
                    OperationType.LessThanOrEqual => LessThanOrEqual
                };
            }

            private static Binary Equal(Outcome left, Outcome right) =>
                new(left == right);

            private static Binary NotEqual(Outcome left, Outcome right) =>
                new(left != right);

            private static Binary GreaterThan(Outcome left, Outcome right) =>
                new(left > right);

            private static Binary GreaterThanOrEqual(Outcome left, Outcome right) =>
                new(left >= right);

            private static Binary LessThan(Outcome left, Outcome right) =>
                new(left < right);

            private static Binary LessThanOrEqual(Outcome left, Outcome right) =>
                new(left <= right);
        }
        
        public static class ProbabilityEvaluation
        {
            public static OperationProbabilityDelegate Get(OperationType operationType)
            {
                EnumValueNotDefinedException.ThrowIfValueNotDefined(operationType);
            
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

            private static OperationProbabilityDelegate Not(OperationProbabilityDelegate @delegate) =>
                (left, right) => @delegate(left, right).Inversed();

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
}
