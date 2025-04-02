using System.Linq;

namespace DiceRoll
{
    /// <summary>
    /// A <see cref="INumeric">numerical node</see> that represents a singular <see cref="int">integer</see> number.
    /// </summary>
    public sealed class Constant : NumericNode
    {
        private readonly int _value;
        
        /// <param name="value">Any <see cref="int">integer</see> number to represent.</param>
        public Constant(int value) 
        {
            _value = value;
        }

        public override Outcome Evaluate() =>
            new(_value);

        public override RollProbabilityDistribution GetProbabilityDistribution() =>
            new(Enumerable.Repeat(new Roll(_value, Probability.Hundred), 1));
    }
}
