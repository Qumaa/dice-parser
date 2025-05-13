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

        public override void Next()
        {
            _assertion.Next();
            _value.Next();
            
            CacheEvaluation(_assertion.Evaluation ? new Optional<Outcome>(_value.Evaluation) : Optional<Outcome>.Empty);
        }

        protected override OptionalRollProbabilityDistribution CreateProbabilityDistribution() =>
            _value.GetProbabilityDistribution()
                .Select(x => new Roll(x.Outcome, x.Probability * _assertion.True))
                .ToOptionalRollProbabilityDistribution();
    }
}
