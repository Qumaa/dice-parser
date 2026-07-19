using System.Linq;

namespace DiceRoll
{
    public sealed class Conditional : Operation
    {
        public readonly IAssertion Assertion;
        public readonly INumeric Value;

        public Conditional(IAssertion condition, INumeric value)
        {
            Assertion = condition;
            Value = value;
        }

        public override void NextEvaluation()
        {
            Assertion.NextEvaluation();
            Value.NextEvaluation();
            
            CacheEvaluation(Assertion.CachedEvaluation ? new Optional<Outcome>(Value.CachedEvaluation) : Optional<Outcome>.Empty);
        }

        protected override OptionalRollProbabilityDistribution CreateProbabilityDistribution() =>
            Value.GetProbabilityDistribution()
                .Select(x => new Roll(x.Outcome, x.Probability * Assertion.True))
                .ToOptionalRollProbabilityDistribution();
    }
}
