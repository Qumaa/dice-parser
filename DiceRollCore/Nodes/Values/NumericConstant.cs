namespace DiceRoll
{
    public sealed class NumericConstant : Numeric
    {
        public int Value => CachedEvaluation.Value;
        
        public NumericConstant(int value) 
        {
            CacheEvaluation(value);
        }

        public override void NextEvaluation() { }
        
        protected override RollProbabilityDistribution CreateProbabilityDistribution() =>
            new(CachedEvaluation);
    }
}
