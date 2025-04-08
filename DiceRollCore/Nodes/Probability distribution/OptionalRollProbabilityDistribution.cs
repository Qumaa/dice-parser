using System.Collections.Generic;
using System.Linq;

namespace DiceRoll
{
    public sealed class OptionalRollProbabilityDistribution : ProbabilityDistribution<OptionalRoll>
    {
        public readonly Probability False;
        public readonly Outcome Min;
        public readonly Outcome Max;

        /// <inheritdoc cref="ProbabilityDistribution{T}(IEnumerable{T})"/>
        public OptionalRollProbabilityDistribution(IEnumerable<OptionalRoll> probabilities) : base(probabilities)
        {
            Init(out Min, out Max, out False);
        }

        private void Init(out Outcome min, out Outcome max, out Probability ofFalse)
        {
            if (!ContainsAtLeastOneElement(out IEnumerator<OptionalRoll> enumerator))
            {
                min = max = default;
                ofFalse = default;
                return;
            }

            if (!MoveToFirstExistingRoll(enumerator, out min, out max, out ofFalse))
                return;

            AccumulateMinMax(ref min, ref max, enumerator);
            
            enumerator.Dispose();
        }

        private bool ContainsAtLeastOneElement(out IEnumerator<OptionalRoll> enumerator) =>
            (enumerator = GetEnumerator()).MoveNext();

        private static bool MoveToFirstExistingRoll(IEnumerator<OptionalRoll> enumerator, out Outcome min,
            out Outcome max, out Probability ofFalse)
        {
            Outcome outcome;
            ofFalse = Probability.Zero;
            
            while (!enumerator.Current.Outcome.Exists(out outcome))
            {
                ofFalse += enumerator.Current.Probability;

                if (enumerator.MoveNext())
                    continue;

                min = max = default;
                return false;
            }

            min = max = outcome;
            return true;
        }

        private static void AccumulateMinMax(ref Outcome min, ref Outcome max, IEnumerator<OptionalRoll> enumerator)
        {
            while (enumerator.MoveNext())
            {
                if (!enumerator.Current.Outcome.Exists(out Outcome outcome))
                    continue;

                if (outcome < min)
                    min = outcome;

                if (outcome > max)
                    max = outcome;
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
                .Select(x => new OptionalRoll(x))
                .Prepend(new OptionalRoll(enumeratedRolls.Aggregate(Probability.Hundred,
                    (probability, roll) => probability - roll.Probability)
                    )
                )
            );
        }

        public static LogicalProbabilityDistribution AsLogical(this OptionalRollProbabilityDistribution distribution) =>
            new(distribution.False.Inversed());
    }
}
