using System.Collections.Generic;

namespace DiceRoll.Expressions
{
    public sealed class Composite : IAnalyzable
    {
        private readonly IAnalyzable _composed;
        public Composite(IEnumerable<IAnalyzable> sequence, Composer composer)
        {
            _composed = composer.Compose(sequence);
        }

        public Outcome Evaluate() =>
            _composed.Evaluate();

        public ProbabilityDistribution GetProbabilityDistribution() =>
            _composed.GetProbabilityDistribution();
    }
}
