using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Expressions
{
    public sealed class ProbabilityDistribution : IEnumerable<Roll>
    {
        public readonly Outcome Min;
        public readonly Outcome Max;
        
        private readonly IEnumerable<Roll> _probabilities;

        public ProbabilityDistribution(IEnumerable<Roll> probabilities)
        {
            _probabilities = probabilities;
            
            Init(out Min, out Max);
        }

        private void Init(out Outcome min, out Outcome max)
        {
            using IEnumerator<Roll> enumerator = _probabilities.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                min = max = default;
                return;
            }

            min = max = enumerator.Current.Outcome;

            while (enumerator.MoveNext())
            {
                Outcome current = enumerator.Current.Outcome;

                if (current.Value < min.Value)
                    min = current;

                if (current.Value > max.Value)
                    max = current;
            }
        }

        public ProbabilityDistribution Combine(ProbabilityDistribution other)
        {
            int minValue = Min.Value + other.Min.Value;
            int maxValue = Max.Value + other.Max.Value;

            double[] newProbabilities = new double[maxValue - minValue + 1];

            foreach (Roll thisRoll in _probabilities)
            foreach (Roll otherRoll in other)
            {
                int value = thisRoll.Outcome.Value + otherRoll.Outcome.Value;
                double probability = thisRoll.Probability.Value * otherRoll.Probability.Value;

                newProbabilities[value - minValue] += probability;
            }

            return new ProbabilityDistribution(newProbabilities
                .Select(
                    (d, i) => new Roll(i + minValue, d)
                )
            );
        }

        public IEnumerator<Roll> GetEnumerator() =>
            _probabilities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}
