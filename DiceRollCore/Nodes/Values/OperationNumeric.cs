using System.Linq;

namespace DiceRoll
{
    public sealed class Conditional : IOperation
    {
        private readonly IAssertion _assertion;
        private readonly INumeric _value;

        public Conditional(IAssertion condition, INumeric value)
        {
            _assertion = condition;
            _value = value;
        }

        public OptionalRollProbabilityDistribution GetProbabilityDistribution() =>
            _value.GetProbabilityDistribution()
                .Select(x => new Roll(x.Outcome, x.Probability * _assertion.True))
                .ToOptionalRollProbabilityDistribution();

        public IAssertion AsAssertion() =>
            _assertion;

        public void Visit(INodeVisitor visitor) =>
            visitor.ForOperation(this);

        public Optional<Outcome> Evaluate() =>
            _assertion.Evaluate() ? new Optional<Outcome>(_value.Evaluate()) : Optional<Outcome>.Empty;
    }
}
