namespace DiceRoll
{
    /// <summary>
    /// A <see cref="INumeric">numerical node</see> that represents a singular <see cref="int">integer</see> number.
    /// </summary>
    public sealed class Constant : Numeric
    {
        private readonly int _value;
        
        /// <param name="value">Any <see cref="int">integer</see> number to represent.</param>
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
