namespace DiceRoll
{
    public sealed class CDFTable
    {
        private readonly Outcome _min;
        private readonly Outcome _max;
        private readonly Probability[] _probabilities;
        private readonly Probability[] _cdfProbabilities;

        public CDFTable(RollProbabilityDistribution distribution)
        {
            ArgumentNullException.ThrowIfNull(distribution);
            
            _min = distribution.Min;
            _max = distribution.Max;
                
            _probabilities = distribution.Select(x => x.Probability).ToArray();
            _cdfProbabilities = BuildCDF(_probabilities);
        }

        public Probability EqualTo(Outcome outcome)
        {
            if (outcome < _min || outcome > _max)
                return Probability.Zero;

            return _probabilities[OutcomeToIndex(outcome)];
        }

        public Probability LessThanOrEqualTo(Outcome outcome)
        {
            if (outcome > _max)
                return Probability.Zero;
            
            if (outcome < _min)
                return Probability.Hundred;

            return _cdfProbabilities[OutcomeToIndex(outcome)];
        }

        public Probability GreaterThanOrEqualTo(Outcome outcome)
        {
            if (outcome > _max)
                return Probability.Hundred;
            
            if (outcome < _min)
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
                cdf[i] = probabilities[i] + cdf[i - 1];
                
            return cdf;
        }
    }

    public static class CDFTableExtensions
    {
        public static Probability NotEqualTo(this CDFTable cdfTable, Outcome outcome) =>
            cdfTable.EqualTo(outcome).Inversed();

        public static Probability GreaterThan(this CDFTable cdfTable, Outcome outcome) =>
            cdfTable.LessThanOrEqualTo(outcome).Inversed();
        
        public static Probability LessThan(this CDFTable cdfTable, Outcome outcome) =>
            cdfTable.GreaterThanOrEqualTo(outcome).Inversed();
    }
}
