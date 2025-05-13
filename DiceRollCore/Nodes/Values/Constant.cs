namespace DiceRoll
{
    public sealed class Constant : Numeric
    {
        public Constant(int value) 
        {
            CacheEvaluation(new Outcome(value));
        }

        public override void Next() { }

        protected override RollProbabilityDistribution CreateProbabilityDistribution() =>
            new(Evaluation);
    }
}
