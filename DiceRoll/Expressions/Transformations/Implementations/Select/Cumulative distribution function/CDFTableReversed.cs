namespace DiceRoll.Expressions
{
    public sealed class CDFTableReversed : CDFProvider
    {
        public CDFTableReversed(ProbabilityDistribution distribution) : base(distribution, BuildCDF) { }

        protected override bool OutOfBounds(Outcome outcome, out CDF cdf)
        {
            if (outcome.Value > _max.Value)
            {
                cdf = new CDF(Probability.Zero, Probability.Zero);
                return true;
            }
            
            if (outcome.Value < _min.Value)
            {
                cdf = new CDF(Probability.Zero, Probability.Hundred);
                return true;
            }

            cdf = default;
            return false;
        }

        private static Probability[] BuildCDF(Roll[] rolls)
        {
            Probability[] cdf = new Probability[rolls.Length];
            cdf[^1] = rolls[^1].Probability;
                
            for (int i = cdf.Length - 2; i >= 0; i--)
                cdf[i] = new Probability(rolls[i].Probability.Value + cdf[i + 1].Value);
                
            return cdf;
        }
    }
}
