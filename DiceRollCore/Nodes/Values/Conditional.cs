using System.Linq;

namespace DiceRoll
{
    public sealed class Conditional : Operation
    {
        private readonly IAssertion _assertion;
        private readonly INumeric _value;

        public Conditional(IAssertion condition, INumeric value)
        {
            _assertion = condition;
            _value = value;
        }

        protected override OptionalRollProbabilityDistribution CreateProbabilityDistribution() =>
            _value.GetProbabilityDistribution()
                .Select(x => new Roll(x.Outcome, x.Probability * _assertion.True))
                .ToOptionalRollProbabilityDistribution();

        public override Optional<Outcome> Evaluate() =>
            _assertion.Evaluate() ? new Optional<Outcome>(_value.Evaluate()) : Optional<Outcome>.Empty;
    }
}
