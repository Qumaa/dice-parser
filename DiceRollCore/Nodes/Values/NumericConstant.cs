namespace DiceRoll
{
    public sealed class NumericConstant : Numeric
    {
        public NumericConstant(int value) 
        {
            CacheEvaluation(value);
        }

        public override void NextEvaluation() { }
        
        protected override RollProbabilityDistribution CreateProbabilityDistribution() =>
            new(CachedEvaluation);
    }
}
