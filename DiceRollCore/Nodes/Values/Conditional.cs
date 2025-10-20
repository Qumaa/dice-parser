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

        public override void NextEvaluation()
        {
            _assertion.NextEvaluation();
            _value.NextEvaluation();
            
            CacheEvaluation(_assertion.CachedEvaluation ? new Optional<Outcome>(_value.CachedEvaluation) : Optional<Outcome>.Empty);
        }

        protected override OptionalRollProbabilityDistribution CreateProbabilityDistribution() =>
            _value.GetProbabilityDistribution()
                .Select(x => new Roll(x.Outcome, x.Probability * _assertion.True))
                .ToOptionalRollProbabilityDistribution();
    }
}
