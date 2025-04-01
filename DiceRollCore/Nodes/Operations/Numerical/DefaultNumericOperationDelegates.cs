namespace DiceRoll
{
    public static class DefaultNumericOperationDelegates
    {
        public static NumericOperationDelegates Get(NumericOperationType operationType)
        {
            EnumValueNotDefinedException.ThrowIfValueNotDefined(operationType);
            
            return new NumericOperationDelegates(Evaluation.Get(operationType), ProbabilityEvaluation.Get(operationType));
        }

        public static class Evaluation
        {
            public static NumericOperationEvaluationDelegate Get(NumericOperationType operationType) =>
                operationType switch
                {
                    NumericOperationType.Equal => Equal,
                    NumericOperationType.NotEqual => NotEqual,
                    NumericOperationType.GreaterThan => GreaterThan,
                    NumericOperationType.GreaterThanOrEqual => GreaterThanOrEqual,
                    NumericOperationType.LessThan => LessThan,
                    NumericOperationType.LessThanOrEqual => LessThanOrEqual
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
        
        public static class ProbabilityEvaluation
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
