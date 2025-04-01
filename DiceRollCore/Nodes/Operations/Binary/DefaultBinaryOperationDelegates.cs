namespace DiceRoll
{
    public static class DefaultBinaryOperationDelegates
    {
        public static BinaryOperationDelegates Get(BinaryOperationType operationType)
        {
            EnumValueNotDefinedException.ThrowIfValueNotDefined(operationType);
            
            return new BinaryOperationDelegates(Evaluation.Get(operationType), ProbabilityEvaluation.Get(operationType));
        }

        private static class Evaluation
        {
            public static BinaryOperationEvaluationDelegate Get(BinaryOperationType operationType) =>
                operationType switch
                {
                    BinaryOperationType.And => And,
                    BinaryOperationType.Or => Or,
                    BinaryOperationType.Equal => Equal
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
            public static BinaryOperationProbabilityDelegate Get(BinaryOperationType operationType) =>
                operationType switch
                {
                    BinaryOperationType.And => And,
                    BinaryOperationType.Or => Or,
                    BinaryOperationType.Equal => Equal
                };

            private static Probability And(LogicalProbabilityDistribution left, LogicalProbabilityDistribution right) =>
                left.True * right.True;

            private static Probability Or(LogicalProbabilityDistribution left, LogicalProbabilityDistribution right) =>
                left.True + right.True - And(left, right);

            private static Probability Equal(LogicalProbabilityDistribution left, LogicalProbabilityDistribution right) =>
                (left.True + right.True - And(left, right) * 2).Inversed();
        }
    }
}
