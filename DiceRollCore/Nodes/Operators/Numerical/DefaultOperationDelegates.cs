using System.Linq;

namespace DiceRoll
{
    public static class DefaultOperationDelegates
    {
        public static OperationDelegates Get(OperationType operationType)
        {
            EnumValueNotDefinedException.ThrowIfValueNotDefined(operationType);

            return new OperationDelegates(
                Evaluation.Get(operationType),
                RollsProbability.Get(operationType),
                ProbabilityDistribution.Get(operationType)
                );
        }

        private static class Evaluation
        {
            public static OperationEvaluationDelegate Get(OperationType operationType) =>
                operationType switch
                {
                    OperationType.Equal => static (left, right) => Equal(left, right),
                    OperationType.NotEqual => static (left, right) => NotEqual(left, right),
                    OperationType.GreaterThan => static (left, right) => GreaterThan(left, right),
                    OperationType.GreaterThanOrEqual => static (left, right) => GreaterThanOrEqual(left, right),
                    OperationType.LessThan => static (left, right) => LessThan(left, right),
                    OperationType.LessThanOrEqual => static (left, right) => LessThanOrEqual(left, right)
                };

            private static Optional<Outcome> Equal(Outcome left, Outcome right) =>
                ToOptional(left, left == right);

            private static Optional<Outcome> NotEqual(Outcome left, Outcome right) =>
                ToOptional(left, left != right);

            private static Optional<Outcome> GreaterThan(Outcome left, Outcome right) =>
                ToOptional(left, left > right);

            private static Optional<Outcome> GreaterThanOrEqual(Outcome left, Outcome right) =>
                ToOptional(left, left >= right);

            private static Optional<Outcome> LessThan(Outcome left, Outcome right) =>
                ToOptional(left, left < right);

            private static Optional<Outcome> LessThanOrEqual(Outcome left, Outcome right) =>
                ToOptional(left, left <= right);
            
            private static Optional<Outcome> ToOptional(Outcome left, bool result) =>
                result ? new Optional<Outcome>(left) : Optional<Outcome>.Empty;
        }

        private static class ProbabilityDistribution
        {
            private static readonly LogicalProbabilityDistribution _zero = new(Probability.Zero);
            private static readonly LogicalProbabilityDistribution _hundred = new(Probability.Hundred);

            public static AssertionEvaluationDelegate Get(OperationType operationType) =>
                operationType switch
                {
                    OperationType.Equal => static (left, right) => Equal(left, right),
                    OperationType.NotEqual => static (left, right) => NotEqual(left, right),
                    OperationType.GreaterThan => static (left, right) => GreaterThan(left, right),
                    OperationType.GreaterThanOrEqual => static (left, right) => GreaterThanOrEqual(left, right),
                    OperationType.LessThan => static (left, right) => LessThan(left, right),
                    OperationType.LessThanOrEqual => static (left, right) => LessThanOrEqual(left, right)
                };

            private static LogicalProbabilityDistribution Equal(RollProbabilityDistribution left,
                RollProbabilityDistribution right)
            {
                if (left.Max < right.Min || left.Min > right.Max)
                    return _zero;

                CDFTable leftTable = new(left);
                CDFTable rightTable = new(right);

                return new LogicalProbabilityDistribution(
                    left.Intersection(right)
                        .Aggregate(
                            Probability.Zero,
                            (accumulated, outcome) =>
                                accumulated + (leftTable.EqualTo(outcome) * rightTable.EqualTo(outcome))
                            )
                    );
            }

            private static LogicalProbabilityDistribution NotEqual(RollProbabilityDistribution left,
                RollProbabilityDistribution right)
            {
                if (left.Max < right.Min || left.Min > right.Max)
                    return _hundred;

                CDFTable leftTable = new(left);
                CDFTable rightTable = new(right);

                return new LogicalProbabilityDistribution(
                    left.Intersection(right)
                        .Aggregate(
                            Probability.Zero,
                            (accumulated, outcome) =>
                                accumulated + (leftTable.EqualTo(outcome) * rightTable.NotEqualTo(outcome))
                            )
                    );
            }

            private static LogicalProbabilityDistribution GreaterThan(RollProbabilityDistribution left,
                RollProbabilityDistribution right)
            {
                if (left.Max <= right.Min)
                    return _zero;

                if (left.Min > right.Max)
                    return _hundred;

                CDFTable leftTable = new(left);
                CDFTable rightTable = new(right);

                return new LogicalProbabilityDistribution(
                    left.Select(x => x.Outcome)
                        .Aggregate(
                            Probability.Zero,
                            (accumulated, outcome) =>
                                accumulated + (leftTable.EqualTo(outcome) * rightTable.GreaterThan(outcome))
                            )
                    );
            }

            private static LogicalProbabilityDistribution GreaterThanOrEqual(RollProbabilityDistribution left,
                RollProbabilityDistribution right)
            {
                if (left.Max < right.Min)
                    return _zero;

                if (left.Min >= right.Max)
                    return _hundred;

                CDFTable leftTable = new(left);
                CDFTable rightTable = new(right);

                return new LogicalProbabilityDistribution(
                    left.Select(x => x.Outcome)
                        .Aggregate(
                            Probability.Zero,
                            (accumulated, outcome) =>
                                accumulated + (leftTable.EqualTo(outcome) * rightTable.GreaterThanOrEqual(outcome))
                            )
                    );
            }

            private static LogicalProbabilityDistribution LessThan(RollProbabilityDistribution left,
                RollProbabilityDistribution right)
            {
                if (left.Min >= right.Max)
                    return _zero;

                if (left.Max < right.Min)
                    return _hundred;

                CDFTable leftTable = new(left);
                CDFTable rightTable = new(right);

                return new LogicalProbabilityDistribution(
                    left.Select(x => x.Outcome)
                        .Aggregate(
                            Probability.Zero,
                            (accumulated, outcome) =>
                                accumulated + (leftTable.EqualTo(outcome) * rightTable.LessThan(outcome))
                            )
                    );
            }

            private static LogicalProbabilityDistribution LessThanOrEqual(RollProbabilityDistribution left,
                RollProbabilityDistribution right)
            {
                if (left.Min > right.Max)
                    return _zero;

                if (left.Max <= right.Min)
                    return _hundred;

                CDFTable leftTable = new(left);
                CDFTable rightTable = new(right);

                return new LogicalProbabilityDistribution(
                    left.Select(x => x.Outcome)
                        .Aggregate(
                            Probability.Zero,
                            (accumulated, outcome) =>
                                accumulated + (leftTable.EqualTo(outcome) * rightTable.LessThanOrEqual(outcome))
                            )
                    );
            }
        }

        private static class RollsProbability
        {
            public static OperationDistributionDelegate Get(OperationType operationType) =>
                operationType switch
                {
                    OperationType.Equal => Equal,
                    OperationType.NotEqual => NotEqual,
                    OperationType.GreaterThan => GreaterThan,
                    OperationType.GreaterThanOrEqual => GreaterThanOrEqual,
                    OperationType.LessThan => LessThan,
                    OperationType.LessThanOrEqual => LessThanOrEqual,
                };

            private static OptionalRollProbabilityDistribution Equal(RollProbabilityDistribution left,
                RollProbabilityDistribution right)
            {
                CDFTable leftTable = new(left);
                CDFTable rightTable = new(right);

                return left
                    .Where(x => x.Outcome >= right.Min && x.Outcome <= right.Max)
                    .Select(
                        x => new Roll(x.Outcome, leftTable.EqualTo(x.Outcome) * rightTable.EqualTo(x.Outcome)))
                    .Where(x => x.Probability > Probability.Zero)
                    .ToOptionalRollProbabilityDistribution();
            }
            
            private static OptionalRollProbabilityDistribution NotEqual(RollProbabilityDistribution left,
                RollProbabilityDistribution right)
            {
                CDFTable leftTable = new(left);
                CDFTable rightTable = new(right);

                return left
                    .Select(
                        x => new Roll(x.Outcome, leftTable.EqualTo(x.Outcome) * rightTable.NotEqualTo(x.Outcome)))
                    .ToOptionalRollProbabilityDistribution();
            }
            
            private static OptionalRollProbabilityDistribution GreaterThan(RollProbabilityDistribution left,
                RollProbabilityDistribution right)
            {
                CDFTable leftTable = new(left);
                CDFTable rightTable = new(right);

                return left
                    .Where(x => x.Outcome > right.Min)
                    .Select(
                        x => new Roll(
                            x.Outcome,
                            x.Outcome > right.Max ?
                                leftTable.EqualTo(x.Outcome) :
                                leftTable.EqualTo(x.Outcome) * rightTable.LessThan(x.Outcome)
                                )
                        )
                    .ToOptionalRollProbabilityDistribution();
            }

            private static OptionalRollProbabilityDistribution GreaterThanOrEqual(RollProbabilityDistribution left,
                RollProbabilityDistribution right)
            {
                CDFTable leftTable = new(left);
                CDFTable rightTable = new(right);

                return left
                    .Where(x => x.Outcome >= right.Min)
                    .Select(
                        x => new Roll(
                            x.Outcome,
                            x.Outcome >= right.Max ?
                                leftTable.EqualTo(x.Outcome) :
                                leftTable.EqualTo(x.Outcome) * rightTable.LessThanOrEqual(x.Outcome)
                            )
                        )
                    .ToOptionalRollProbabilityDistribution();
            }

            private static OptionalRollProbabilityDistribution LessThan(RollProbabilityDistribution left,
                RollProbabilityDistribution right)
            {
                CDFTable leftTable = new(left);
                CDFTable rightTable = new(right);

                return left
                    .Where(x => x.Outcome < right.Max)
                    .Select(
                        x => new Roll(
                            x.Outcome,
                            x.Outcome < right.Min ?
                                leftTable.EqualTo(x.Outcome) :
                                leftTable.EqualTo(x.Outcome) * rightTable.GreaterThan(x.Outcome)
                            )
                        )
                    .ToOptionalRollProbabilityDistribution();
            }
            
            private static OptionalRollProbabilityDistribution LessThanOrEqual(RollProbabilityDistribution left,
                RollProbabilityDistribution right)
            {
                CDFTable leftTable = new(left);
                CDFTable rightTable = new(right);

                return left
                    .Where(x => x.Outcome <= right.Max)
                    .Select(
                        x => new Roll(
                            x.Outcome,
                            x.Outcome <= right.Min ?
                                leftTable.EqualTo(x.Outcome) :
                                leftTable.EqualTo(x.Outcome) * rightTable.GreaterThanOrEqual(x.Outcome)
                                )
                        )
                    .ToOptionalRollProbabilityDistribution();
            }
        }
    }
}
