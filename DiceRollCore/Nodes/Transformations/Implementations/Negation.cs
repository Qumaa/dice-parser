using System.Linq;

namespace DiceRoll
{
    public sealed class Negation : Transformation
    {
        public Negation(INumeric node) : base(node) { }

        public override void NextEvaluation()
        {
            base.NextEvaluation();
            CacheEvaluation(-Node.CachedEvaluation);
        }

        protected override RollProbabilityDistribution CreateProbabilityDistribution() =>
            Node.GetProbabilityDistribution()
                .Select(x => new Roll(-x.Outcome, x.Probability))
                .ToRollProbabilityDistribution();
    }
}
