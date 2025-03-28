using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Nodes
{
    /// <summary>
    /// A numeric implementation of <see cref="ProbabilityDistribution{T}">ProbabilityDistribution</see>
    /// of type <see cref="Roll"/> with a reasonable naming.
    /// Provides extra numbers-related functionality.
    /// </summary>
    public sealed class RollProbabilityDistribution : ProbabilityDistribution<Roll>
    {
        public readonly Outcome Min;
        public readonly Outcome Max;

        /// <inheritdoc cref="ProbabilityDistribution{T}(IEnumerable{T})"/>
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
