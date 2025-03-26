namespace DiceRoll.Expressions
{
    public sealed class Combination : CommonProbabilityDistributionTransformation
    {
        private readonly CombinationType _combinationType;

        public Combination(RollProbabilityDistribution source, RollProbabilityDistribution other,
            CombinationType combinationType) : base(source, other)
        {
            _combinationType = combinationType;
        }

        protected override Probability[] AllocateProbabilitiesArray(out int valueToIndexOffset)
        {
            int minValue = _source.Min.Value + ApplyCombinationType(_other.Min).Value;
            int maxValue = _source.Max.Value + ApplyCombinationType(_other.Max).Value;

            return new Probability[maxValue - (valueToIndexOffset = minValue) + 1];
        }

        protected override Probability[] GenerateProbabilities(Probability[] probabilities, int valueToIndexOffset)
        {
            foreach (Roll sourceRoll in _source)
            foreach (Roll otherRoll in _other)
            {
                Outcome outcome = sourceRoll.Outcome + ApplyCombinationType(otherRoll.Outcome);
                Probability probability = sourceRoll.Probability * otherRoll.Probability;

                probabilities[outcome.Value - valueToIndexOffset] += probability;
            }

            return probabilities;
        }

        private Outcome ApplyCombinationType(Outcome value) =>
            _combinationType is CombinationType.Subtract ? -value : value;
    }
}
