namespace DiceRoll
{
    public static class DefaultAssertionDelegates
    {
        public static BinaryOperationDelegates Get(BinaryAssertionType assertionType)
        {
            EnumValueNotDefinedException.ThrowIfValueNotDefined(assertionType);

            return new BinaryOperationDelegates(
                Evaluation.Get(assertionType),
                ProbabilityEvaluation.Get(assertionType)
                );
        }

        private static class Evaluation
        {
            public static BinaryAssertionEvaluationDelegate Get(BinaryAssertionType assertionType) =>
                assertionType switch
                {
                    BinaryAssertionType.And => static (left, right) => And(left, right),
                    BinaryAssertionType.Or => static (left, right) => Or(left, right),
                    BinaryAssertionType.Equal => static (left, right) => Equal(left, right)
                };

            private static Binary And(Binary left, Binary right) =>
                left && right;

            private static Binary Or(Binary left, Binary right) =>
                left || right;

            private static Binary Equal(Binary left, Binary right) =>
                (left && right) || !(left || right);
        }

        private static class ProbabilityEvaluation
        {
            public static BinaryAssertionProbabilityDelegate Get(BinaryAssertionType assertionType) =>
                assertionType switch
                {
                    BinaryAssertionType.And => static (left, right) => And(left, right),
                    BinaryAssertionType.Or => static (left, right) => Or(left, right),
                    BinaryAssertionType.Equal => static (left, right) => Equal(left, right)
                };

            private static Probability And(LogicalProbabilityDistribution left, LogicalProbabilityDistribution right) =>
                left.True * right.True;

            private static Probability Or(LogicalProbabilityDistribution left, LogicalProbabilityDistribution right) =>
                (left.True + right.True) - And(left, right);

            private static Probability Equal(LogicalProbabilityDistribution left,
                LogicalProbabilityDistribution right) =>
                ((left.True + right.True) - (And(left, right) * 2)).Inversed();
        }
    }
}
