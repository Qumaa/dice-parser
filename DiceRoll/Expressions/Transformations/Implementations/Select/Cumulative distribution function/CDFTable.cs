namespace DiceRoll.Expressions
{
    public sealed class CDFTable : CDFProvider
    {
        public CDFTable(ProbabilityDistribution distribution) : base(distribution, BuildCDF) { }

        protected override bool OutOfBounds(Outcome outcome, out CDF cdf)
        {
            if (outcome.Value < _min.Value)
            {
                cdf = new CDF(Probability.Zero, Probability.Zero);
                return true;
            }
            
            if (outcome.Value > _max.Value)
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
            cdf[0] = rolls[0].Probability;
                
            for (int i = 1; i < cdf.Length; i++)
                cdf[i] = new Probability(rolls[i].Probability.Value + cdf[i - 1].Value);
                
            return cdf;
        }
    }
}
