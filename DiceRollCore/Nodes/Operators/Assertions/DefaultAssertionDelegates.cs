using DiceRoll.Exceptions;

#pragma warning disable CS8524

namespace DiceRoll
{
    public static class DefaultAssertionDelegates
    {
        public static BinaryOperationDelegates Get(BinaryAssertionType assertionType)
        {
            EnumValueNotDefinedException.ThrowIfValueNotDefined(assertionType);

            return new BinaryOperationDelegates(
                Evaluation.GetDelegate(assertionType),
                ProbabilityEvaluation.GetDelegate(assertionType)
                );
        }

        private static class Evaluation
        {
            public static BinaryAssertionEvaluationDelegate GetDelegate(BinaryAssertionType assertionType) =>
                assertionType switch
                {
                    BinaryAssertionType.And => static (left, right) => And(left, right),
                    BinaryAssertionType.Or => static (left, right) => Or(left, right),
                    BinaryAssertionType.Equal => static (left, right) => Equal(left, right),
                    BinaryAssertionType.NotEqual => static (left, right) => NotEqual(left, right)
                };

            private static Binary And(Binary left, Binary right) =>
                left && right;

            private static Binary Or(Binary left, Binary right) =>
                left || right;

            private static Binary Equal(Binary left, Binary right) =>
                left == right;

            private static Binary NotEqual(Binary left, Binary right) =>
                left != right;
        }

        private static class ProbabilityEvaluation
        {
            public static BinaryAssertionProbabilityDelegate GetDelegate(BinaryAssertionType assertionType) =>
                assertionType switch
                {
                    BinaryAssertionType.And => static (left, right) => And(left, right),
                    BinaryAssertionType.Or => static (left, right) => Or(left, right),
                    BinaryAssertionType.Equal => static (left, right) => Equal(left, right),
                    BinaryAssertionType.NotEqual => static (left, right) => NotEqual(left, right)
                };

            private static Probability And(LogicalProbabilityDistribution left, LogicalProbabilityDistribution right) =>
                left.True * right.True;

            private static Probability Or(LogicalProbabilityDistribution left, LogicalProbabilityDistribution right) =>
                (left.True + right.True) - And(left, right);

            private static Probability Equal(LogicalProbabilityDistribution left,
                LogicalProbabilityDistribution right) =>
                NotEqual(left, right).Inversed();

            private static Probability NotEqual(LogicalProbabilityDistribution left,
                LogicalProbabilityDistribution right) =>
                (left.True + right.True) - (And(left, right) * 2);
        }
    }
}
