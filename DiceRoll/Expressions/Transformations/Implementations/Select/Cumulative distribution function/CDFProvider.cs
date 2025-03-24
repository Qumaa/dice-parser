using System.Linq;

namespace DiceRoll.Expressions
{
    public abstract class CDFProvider
    {
        protected readonly Outcome _min;
        protected readonly Outcome _max;
        private readonly Roll[] _rolls;
        private readonly Probability[] _cdfProbabilities;

        protected CDFProvider(ProbabilityDistribution distribution, CDFBuilder cdfBuilder)
        {
            _min = distribution.Min;
            _max = distribution.Max;
                
            _rolls = distribution.ToArray();
            _cdfProbabilities = cdfBuilder.Invoke(_rolls);
        }
        
        public CDF ForOutcome(Outcome outcome)
        {
            if (OutOfBounds(outcome, out CDF cdf))
                return cdf;
                
            int i = outcome.Value - _min.Value;

            return new CDF(_rolls[i].Probability, _cdfProbabilities[i]);
        }

        protected abstract bool OutOfBounds(Outcome outcome, out CDF cdf);
    }
}
