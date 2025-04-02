using System.Linq;

namespace DiceRoll
{
    public sealed class Conditional : IConditional
    {
        private readonly IOperation _condition;
        private readonly IAnalyzable _value;
        
        public Conditional(IOperation condition, IAnalyzable value)
        {
            _condition = condition;
            _value = value;
        }

        public Optional<Outcome> Evaluate() =>
            _condition.Evaluate() ? new Optional<Outcome>(_value.Evaluate()) : Optional<Outcome>.Empty;

        public RollProbabilityDistribution GetProbabilityDistribution()
        {
            Probability ofTrue = _condition.GetProbabilityDistribution().True;

            return _value.GetProbabilityDistribution()
                .Select(x => new Roll(x.Outcome, x.Probability * ofTrue))
                .ToRollProbabilityDistribution();
        }
    }
}
