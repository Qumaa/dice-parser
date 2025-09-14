namespace DiceRoll
{
    public sealed class NumericConstant : Numeric
    {
        public NumericConstant(int value) 
        {
            CacheEvaluation(new Outcome(value));
        }

        public override void Next() { }

        protected override RollProbabilityDistribution CreateProbabilityDistribution() =>
            new(Evaluation);
    }
}
