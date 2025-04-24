using System.Collections.Generic;
using System.Linq;

namespace DiceRoll
{
    public sealed class RollProbabilityDistribution : ProbabilityDistribution<Roll>
    {
        public readonly Outcome Min;
        public readonly Outcome Max;

        public RollProbabilityDistribution(IEnumerable<Roll> rolls) : base(rolls)
        {
            Init(out Min, out Max);
        }

        public RollProbabilityDistribution(Outcome outcome) : base(new[] { new Roll(outcome, Probability.Hundred) })
        {
            Min = Max = outcome;
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
        public static RollProbabilityDistribution ToRollProbabilityDistribution(this IEnumerable<Roll> probabilities) => 
            new(probabilities);
        
        public static IEnumerable<Outcome> Intersection(this RollProbabilityDistribution source,
            RollProbabilityDistribution other) =>
            source
                .Select(x => x.Outcome)
                .Where(x => x >= other.Min && x <= other.Max)
                .Intersect(other.Select(x => x.Outcome), Outcome.EqualityComparer);
        
        public static IEnumerable<Outcome> Union(this RollProbabilityDistribution source, 
            RollProbabilityDistribution other) =>
            source
                .Select(x => x.Outcome)
                .Union(other.Select(x => x.Outcome), Outcome.EqualityComparer);
        
        public static IEnumerable<Outcome> Distinct(this RollProbabilityDistribution source) =>
            source.Select(x => x.Outcome).Distinct(Outcome.EqualityComparer);
    }
}
