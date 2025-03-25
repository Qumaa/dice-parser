using System;
using System.Linq;

namespace DiceRoll.Expressions
{
    public sealed class CDFTable
    {
        private readonly Outcome _min;
        private readonly Outcome _max;
        private readonly Probability[] _probabilities;
        private readonly Probability[] _cdfProbabilities;

        public CDFTable(RollProbabilityDistribution distribution)
        {
            _min = distribution.Min;
            _max = distribution.Max;
                
            _probabilities = distribution.Select(x => x.Probability).ToArray();
            _cdfProbabilities = BuildCDF(_probabilities);
        }

        public Probability EqualTo(Outcome outcome)
        {
            if (outcome.Value < _min.Value || outcome.Value > _max.Value)
                return Probability.Zero;

            return _probabilities[OutcomeToIndex(outcome)];
        }

        public Probability LessOrEqualThan(Outcome outcome)
        {
            if (outcome.Value > _max.Value)
                return Probability.Zero;
            
            if (outcome.Value < _min.Value)
                return Probability.Hundred;

            return _cdfProbabilities[OutcomeToIndex(outcome)];
        }

        public Probability GreaterOrEqualThan(Outcome outcome)
        {
            if (outcome.Value > _max.Value)
                return Probability.Hundred;
            
            if (outcome.Value < _min.Value)
                return Probability.Zero;

            return _cdfProbabilities[OutcomeToIndex(outcome, true)];
        }
        
        private Index OutcomeToIndex(Outcome outcome, bool inverse = false) =>
            inverse ? 
                new Index(outcome.Value - _min.Value + 1, true) :
                new Index(outcome.Value - _min.Value);

        private static Probability[] BuildCDF(Probability[] probabilities)
        {
            Probability[] cdf = new Probability[probabilities.Length];
            cdf[0] = probabilities[0];
                
            for (int i = 1; i < cdf.Length; i++)
                cdf[i] = new Probability(probabilities[i].Value + cdf[i - 1].Value);
                
            return cdf;
        }
    }

    public static class CDFTableExtensions
    {
        public static Probability NotEqualTo(this CDFTable cdfTable, Outcome outcome) =>
            cdfTable.EqualTo(outcome).Inversed();

        public static Probability GreaterThan(this CDFTable cdfTable, Outcome outcome) =>
            cdfTable.LessOrEqualThan(outcome).Inversed();
        
        public static Probability LessThan(this CDFTable cdfTable, Outcome outcome) =>
            cdfTable.GreaterOrEqualThan(outcome).Inversed();
    }
}
