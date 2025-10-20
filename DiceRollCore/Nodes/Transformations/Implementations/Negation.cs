using System.Linq;

namespace DiceRoll
{
    public sealed class Negation : Transformation
    {
        public Negation(INumeric node) : base(node) { }

        public override void NextEvaluation()
        {
            base.NextEvaluation();
            CacheEvaluation(-_source.CachedEvaluation);
        }

        protected override RollProbabilityDistribution CreateProbabilityDistribution() =>
            _source.GetProbabilityDistribution()
                .Select(x => new Roll(-x.Outcome, x.Probability))
                .ToRollProbabilityDistribution();
    }
}
