using System.Collections;
using System.Collections.Generic;

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

        public IEnumerator<Roll> GetEnumerator() =>
            _probabilities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}
