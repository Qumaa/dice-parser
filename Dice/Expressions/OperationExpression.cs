using System;
using System.Linq;

namespace Dice.Expressions
{
    public sealed class OperationExpression : IExpression<Probability>
    {
        private readonly IAnalyzable _left;
        private readonly IAnalyzable _right;
        private readonly OperationType _operationType;

        public OperationExpression(IAnalyzable left, OperationType operationType, IAnalyzable right)
        {
            _left = left;
            _operationType = operationType;
            _right = right;
        }

        public Probability Evaluate() =>
            GetDelegate(_operationType).Invoke(_left.GetProbabilityData(), _right.GetProbabilityData());

        private static OperationDelegate GetDelegate(OperationType operationType)
        {
            return operationType switch
            {
                OperationType.Equal => Operations.Equal,
                OperationType.NotEqual => Operations.Not(Operations.Equal),
                OperationType.GreaterThan => Operations.GreaterThan,
                OperationType.GreaterThanOrEqual => Operations.Not(Operations.LessThan),
                OperationType.LessThan => Operations.LessThan,
                OperationType.LessThanOrEqual => Operations.Not(Operations.GreaterThan),
                _ => throw new ArgumentOutOfRangeException(nameof(operationType), operationType, null)
            };
        }

        private static class Operations
        {
            
            public static OperationDelegate Not(OperationDelegate operationDelegate) =>
                (left, right) => operationDelegate(left, right).Inversed();

            public static Probability Equal(ProbabilityDistribution left, ProbabilityDistribution right)
            {
                if (left.Max.Value < right.Min.Value || right.Max.Value < left.Min.Value)
                    return Probability.Zero;
                
                return Evaluate(left, right, (leftRoll, rightRoll) => leftRoll.Value == rightRoll.Value);
            }

            public static Probability GreaterThan(ProbabilityDistribution left, ProbabilityDistribution right)
            {
                if (left.Max.Value < right.Min.Value)
                    return Probability.Zero;

                return Evaluate(left, right, (leftRoll, rightRoll) => leftRoll.Value > rightRoll.Value);
            }

            public static Probability LessThan(ProbabilityDistribution left, ProbabilityDistribution right)
            {
                if ( right.Max.Value < left.Min.Value)
                    return Probability.Zero;
                
                return Evaluate(left, right, (leftRoll, rightRoll) => leftRoll.Value < rightRoll.Value);
            }
        }

        private static Probability Evaluate(ProbabilityDistribution left, ProbabilityDistribution right,
            Func<Outcome, Outcome, bool> predicate) =>
            new(left
                .SelectMany(_ => right, (left, right) => new { left, right })
                .Select(rolls => (rolls, probability: rolls.left.Probability.Value * rolls.right.Probability.Value))
                .Where(t => predicate(t.rolls.left.Outcome, t.rolls.right.Outcome))
                .Select(t => t.probability)
                .Sum());

        private delegate Probability OperationDelegate(ProbabilityDistribution left, ProbabilityDistribution right);
    }
}
