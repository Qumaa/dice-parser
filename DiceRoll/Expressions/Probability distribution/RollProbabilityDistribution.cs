using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Expressions
{
    public sealed class RollProbabilityDistribution : ProbabilityDistribution<Roll>
    {
        public readonly Outcome Min;
        public readonly Outcome Max;

        public RollProbabilityDistribution(IEnumerable<Roll> probabilities) : base(probabilities)
        {
            Init(out Min, out Max);
        }

        private void Init(out Outcome min, out Outcome max)
        {
            using IEnumerator<Roll> enumerator = GetEnumerator();

            if (!enumerator.MoveNext())
            {
                min = max = default;
                return;
            }

            min = max = enumerator.Current.Outcome;

            while (enumerator.MoveNext())
            {
                Outcome current = enumerator.Current.Outcome;

                if (current < min)
                    min = current;

                if (current > max)
                    max = current;
            }
        }
    }

    public static class RollProbabilityDistributionExtensions
    {
        public static IEnumerable<Outcome> Intersection(this RollProbabilityDistribution source,
            RollProbabilityDistribution other) =>
            source
                .Select(x => x.Outcome)
                .Where(x => x >= other.Min && x <= other.Max);
    }
}
