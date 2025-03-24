using System.Linq;

namespace DiceRoll.Expressions
{
    public sealed class Combination : ProbabilityDistributionTransformation
    {
        private readonly ProbabilityDistribution _other;
        private readonly CombinationType _combinationType;

        public Combination(ProbabilityDistribution source, ProbabilityDistribution other,
            CombinationType combinationType) : base(source)
        {
            _other = other;
            _combinationType = combinationType;
        }

        public override ProbabilityDistribution Evaluate()
        {
            int minValue = _source.Min.Value + ApplyCombinationType(_other.Min.Value);
            int maxValue = _source.Max.Value + ApplyCombinationType(_other.Max.Value);

            double[] newProbabilities = new double[maxValue - minValue + 1];

            foreach (Roll sourceRoll in _source)
            foreach (Roll otherRoll in _other)
            {
                int value = sourceRoll.Outcome.Value + ApplyCombinationType(otherRoll.Outcome.Value);
                double probability = sourceRoll.Probability.Value * otherRoll.Probability.Value;

                newProbabilities[value - minValue] += probability;
            }

            return new ProbabilityDistribution(newProbabilities
                .Select(
                    (d, i) => new Roll(i + minValue, d)
                )
            );
        }

        private int ApplyCombinationType(int value) =>
            _combinationType is CombinationType.Subtract ? -value : value;
    }
}
