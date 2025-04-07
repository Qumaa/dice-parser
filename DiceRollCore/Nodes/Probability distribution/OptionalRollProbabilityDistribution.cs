using System.Collections.Generic;
using System.Linq;

namespace DiceRoll
{
    public sealed class OptionalRollProbabilityDistribution : ProbabilityDistribution<OptionalRoll>
    {
        public readonly Outcome Min;
        public readonly Outcome Max;

        /// <inheritdoc cref="ProbabilityDistribution{T}(IEnumerable{T})"/>
        public OptionalRollProbabilityDistribution(IEnumerable<OptionalRoll> probabilities) : base(probabilities)
        {
            Init(out Min, out Max);
        }

        private void Init(out Outcome min, out Outcome max)
        {
            using IEnumerator<OptionalRoll> enumerator = GetEnumerator();

            if (!enumerator.MoveNext())
            {
                min = max = default;
                return;
            }

            Outcome outcome;
            while (!enumerator.Current.Outcome.Exists(out outcome) && enumerator.MoveNext());
            min = max = outcome;

            while (enumerator.MoveNext())
            {
                if (!enumerator.Current.Outcome.Exists(out outcome))
                    continue;
                
                Outcome current = outcome;

                if (current < min)
                    min = current;

                if (current > max)
                    max = current;
            }
        }
    }

    public static class OptionalRollProbabilityDistributionExtensions
    {
        public static OptionalRollProbabilityDistribution ToOptionalRollProbabilityDistribution(this IEnumerable<OptionalRoll> rolls) =>
            new(rolls);

        public static OptionalRollProbabilityDistribution ToOptionalRollProbabilityDistribution(this IEnumerable<Roll> rolls)
        {
            Roll[] enumeratedRolls = rolls as Roll[] ?? rolls.ToArray();

            return new OptionalRollProbabilityDistribution(enumeratedRolls
                .Select(x => new OptionalRoll(x.Outcome, x.Probability))
                .Prepend(new OptionalRoll(enumeratedRolls.Aggregate(Probability.Hundred,
                    (probability, roll) => probability - roll.Probability)
                    )
                )
            );
        }
    }
}
