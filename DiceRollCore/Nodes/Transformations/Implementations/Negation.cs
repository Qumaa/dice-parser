using System.Linq;

namespace DiceRoll
{
    public sealed class Negation : Transformation
    {
        public Negation(INumeric source) : base(source) { }

        public override void NextEvaluation()
        {
            base.NextEvaluation();
            CacheEvaluation(-Source.CachedEvaluation);
        }

        protected override RollProbabilityDistribution CreateProbabilityDistribution() =>
            Source.GetProbabilityDistribution()
                .Select(x => new Roll(-x.Outcome, x.Probability))
                .ToRollProbabilityDistribution();
    }
}
