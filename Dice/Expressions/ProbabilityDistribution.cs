using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dice.Expressions
{
    public sealed class ProbabilityDistribution : IEnumerable<OutcomeProbability>
    {
        public readonly Outcome Min;
        public readonly Outcome Max;
        
        private readonly IEnumerable<OutcomeProbability> _probabilities;

        public ProbabilityDistribution(IEnumerable<OutcomeProbability> probabilities)
        {
            _probabilities = probabilities;
            
            Init(out Min, out Max);
        }

        private void Init(out Outcome min, out Outcome max)
        {
            using IEnumerator<OutcomeProbability> enumerator = _probabilities.GetEnumerator();

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

            foreach (OutcomeProbability thisRoll in _probabilities)
            foreach (OutcomeProbability otherRoll in other)
            {
                int value = thisRoll.Outcome.Value + otherRoll.Outcome.Value;
                double probability = thisRoll.Probability.Value * otherRoll.Probability.Value;

                newProbabilities[value - minValue] += probability;
            }

            return new ProbabilityDistribution(newProbabilities
                .Select(
                    (d, i) => (OutcomeProbability) new(i + minValue, d)
                )
            );
        }

        public IEnumerator<OutcomeProbability> GetEnumerator() =>
            _probabilities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public static ProbabilityDistribution OfDice(int faces, int count = 1)
        {
            Probability probability = new(1d / faces);

            ProbabilityDistribution distribution = new(Enumerable.Range(1, faces)
                .Select(index => new Outcome(index))
                .Select(rollResult => new OutcomeProbability(rollResult, probability)));

            if (count <= 1)
                goto ret;

            for (int i = 1; i < count; i++)
                distribution = distribution.Combine(distribution);

            ret:
            return distribution;
        }

        public static ProbabilityDistribution OfConstant(int value) =>
            new(Enumerable.Repeat(new OutcomeProbability(value, Probability.Hundred), 1));
    }
}
