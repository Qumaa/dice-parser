namespace DiceRoll
{
    public sealed class Constant : Numeric
    {
        private readonly int _value;

        public override Outcome Evaluation => new(_value);

        public Constant(int value) 
        {
            _value = value;
        }

        public override void Next() { }

        protected override RollProbabilityDistribution CreateProbabilityDistribution() =>
            new(new Outcome(_value));
    }
}
