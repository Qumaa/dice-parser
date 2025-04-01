namespace DiceRoll
{
    /// <summary>
    /// Merges two arbitrary <see cref="IAnalyzable">numeric nodes</see> by either adding or subtracting their
    /// <see cref="Outcome"/> and provides an updated
    /// <see cref="RollProbabilityDistribution">probability distribution</see> of the results.
    /// </summary>
    /// <seealso cref="CombinationType"/>
    public sealed class Combination : MergeTransformation
    {
        private readonly CombinationType _combinationType;

        /// <inheritdoc cref="MergeTransformation(IAnalyzable, IAnalyzable)"/>
        /// <param name="combinationType">The type of combination.</param>
        /// <exception cref="EnumValueNotDefinedException">
        /// When <paramref name="combinationType"/> holds a not defined value.
        /// </exception>
        public Combination(IAnalyzable source, IAnalyzable other, CombinationType combinationType) : base(source, other)
        {
            if (IsDivision(combinationType))
                ZeroDivisorException.ThrowIfAnyZero(_other.GetProbabilityDistribution());
            EnumValueNotDefinedException.ThrowIfValueNotDefined(combinationType);
            
            _combinationType = combinationType;
        }

        public override Outcome Evaluate() =>
            Combine(_source.Evaluate(), _other.Evaluate());

        public override RollProbabilityDistribution GetProbabilityDistribution()
        {
            RollProbabilityDistribution source = _source.GetProbabilityDistribution();
            RollProbabilityDistribution other = _other.GetProbabilityDistribution();

            Dictionary<Outcome, Probability> probabilities = new();
            
            foreach (Roll sourceRoll in source)
            foreach (Roll otherRoll in other)
            {
                Outcome outcome = Combine(sourceRoll.Outcome, otherRoll.Outcome);
                Probability probability = sourceRoll.Probability * otherRoll.Probability;

                if (!probabilities.TryAdd(outcome, probability))
                    probabilities[outcome] += probability;
            }

            return new RollProbabilityDistribution(probabilities
                .Select(x => new Roll(x.Key, x.Value))
                .OrderBy(x => x.Outcome)
            );
        }

        private Outcome Combine(Outcome left, Outcome right) =>
            _combinationType switch
            {
                CombinationType.Add => left + right,
                CombinationType.Subtract => left - right,
                CombinationType.Multiply => left * right,
                CombinationType.DivideRoundDownwards => left / right,
                CombinationType.DivideRoundUpwards => DivideRoundUpwards(left, right)
            };

        private static bool IsDivision(CombinationType combinationType) =>
            combinationType is CombinationType.DivideRoundDownwards or CombinationType.DivideRoundUpwards;

        private static Outcome DivideRoundUpwards(Outcome left, Outcome right)
        {
            Outcome outcome = left / right;
            
            return outcome * right < left ? outcome + 1 : outcome;
        }
    }
}
