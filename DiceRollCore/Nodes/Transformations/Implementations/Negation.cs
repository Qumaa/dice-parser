using System.Linq;

namespace DiceRoll
{
    public sealed class Negation : Transformation
    {
        public Negation(INumeric node) : base(node) { }

        protected override Outcome GetNextEvaluation() =>
            -_source.Evaluate();

        protected override RollProbabilityDistribution CreateProbabilityDistribution() =>
            _source.GetProbabilityDistribution()
                .Select(x => new Roll(-x.Outcome, x.Probability))
                .ToRollProbabilityDistribution();
    }
}
