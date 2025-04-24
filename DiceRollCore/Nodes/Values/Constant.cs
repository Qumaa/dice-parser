namespace DiceRoll
{
    public sealed class Constant : Numeric
    {
        private readonly int _value;
        
        public Constant(int value) 
        {
            _value = value;
        }

        public override Outcome Evaluate() =>
            new(_value);

        protected override RollProbabilityDistribution CreateProbabilityDistribution() =>
            new(new Outcome(_value));
    }
}
