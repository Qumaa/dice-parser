using System.Linq;

namespace DiceRoll.Expressions
{
    public sealed class Constant : IAnalyzable
    {
        private readonly int _value;

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
