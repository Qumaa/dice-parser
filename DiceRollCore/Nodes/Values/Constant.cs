namespace DiceRoll
{
    /// <summary>
    /// A <see cref="IAnalyzable">numerical node</see> that represents a singular <see cref="int">integer</see> number.
    /// </summary>
    public sealed class Constant : IAnalyzable
    {
        private readonly int _value;
        
        /// <param name="value">Any <see cref="int">integer</see> number to represent.</param>
        public Constant(int value) 
        {
            _value = value;
        }

        public Outcome Evaluate() =>
            new(_value);

        public RollProbabilityDistribution GetProbabilityDistribution() =>
            new(Enumerable.Repeat(new Roll(_value, Probability.Hundred), 1));
    }
}
