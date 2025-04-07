using System.Linq;

namespace DiceRoll
{
    public static class DefaultNumericOperationDelegates
    {
        public static NumericOperationDelegates Get(NumericOperationType operationType)
        {
            EnumValueNotDefinedException.ThrowIfValueNotDefined(operationType);

            return new NumericOperationDelegates(
                Evaluation.Get(operationType),
                ProbabilityDistribution.Get(operationType),
                RollsProbability.Get(operationType));
        }

        public static class Evaluation
        {
            public static NumericOperationEvaluationDelegate Get(NumericOperationType operationType) =>
                operationType switch
                {
                    NumericOperationType.Equal => static (left, right) => Equal(left, right),
                    NumericOperationType.NotEqual => static (left, right) => NotEqual(left, right),
                    NumericOperationType.GreaterThan => static (left, right) => GreaterThan(left, right),
                    NumericOperationType.GreaterThanOrEqual => static (left, right) => GreaterThanOrEqual(left, right),
                    NumericOperationType.LessThan => static (left, right) => LessThan(left, right),
                    NumericOperationType.LessThanOrEqual => static (left, right) => LessThanOrEqual(left, right)
                };

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

        public static class ProbabilityDistribution
        {
            public static NumericOperationProbabilityDelegate Get(NumericOperationType operationType) =>
                operationType switch
                {
                    NumericOperationType.Equal => Equal,
                    NumericOperationType.NotEqual => Not(Equal),
                    NumericOperationType.GreaterThan => GreaterThan,
                    NumericOperationType.GreaterThanOrEqual => Not(LessThan),
                    NumericOperationType.LessThan => LessThan,
                    NumericOperationType.LessThanOrEqual => Not(GreaterThan)
                };

            private static NumericOperationProbabilityDelegate Not(NumericOperationProbabilityDelegate @delegate) =>
                (left, right) => @delegate(left, right).Inversed();

            private static Probability Equal(RollProbabilityDistribution left, RollProbabilityDistribution right)
            {
                if (left.Max < right.Min || left.Min > right.Max)
                    return Probability.Zero;

                CDFTable leftTable = new(left);
                CDFTable rightTable = new(right);

                return left.Intersection(right)
                    .Aggregate(
                        Probability.Zero,
                        (accumulated, outcome) =>
                            accumulated + (leftTable.EqualTo(outcome) * rightTable.EqualTo(outcome)));
            }

            private static Probability GreaterThan(RollProbabilityDistribution left, RollProbabilityDistribution right)
            {
                if (left.Max <= right.Min)
                    return Probability.Zero;

                CDFTable leftTable = new(left);
                CDFTable rightTable = new(right);

                return left.Select(x => x.Outcome)
                    .Aggregate(
                        Probability.Zero,
                        (accumulated, outcome) =>
                            accumulated + (leftTable.EqualTo(outcome) * rightTable.GreaterThan(outcome)));
            }

            private static Probability LessThan(RollProbabilityDistribution left, RollProbabilityDistribution right)
            {
                if (left.Min >= right.Max)
                    return Probability.Zero;

                CDFTable leftTable = new(left);
                CDFTable rightTable = new(right);

                return left.Select(x => x.Outcome)
                    .Aggregate(
                        Probability.Zero,
                        (accumulated, outcome) =>
                            accumulated + (leftTable.EqualTo(outcome) * rightTable.LessThan(outcome)));
            }
        }

        public static class RollsProbability
        {
            public static NumericOperationOptionalRollsDelegate Get(NumericOperationType operationType) =>
                (left, right) => FromEvaluationDelegate(left, right, Evaluation.Get(operationType));

            private static OptionalRollProbabilityDistribution FromEvaluationDelegate(RollProbabilityDistribution left,
                RollProbabilityDistribution right, NumericOperationEvaluationDelegate evaluationDelegate) =>
                left.SelectMany(_ => right, (l, r) => (l, r))
                    .Where(x => evaluationDelegate(x.l.Outcome, x.r.Outcome).Value)
                    .Select(x => (l: new Roll(x.l.Outcome, x.l.Probability * x.r.Probability), x.r))
                    .GroupBy(
                        x => x.l.Outcome,
                        (o, x) =>
                            new Roll(
                                o,
                                x.Aggregate(
                                    Probability.Zero,
                                    (probability, tuple) => probability + tuple.l.Probability)
                                )
                        )
                    .ToOptionalRollProbabilityDistribution();
        }
    }
}
