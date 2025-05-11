using System.Linq;

namespace DiceRoll
{
    public sealed class Negation : Transformation
    {
        public override Outcome Evaluation => -_source.Evaluation;
        
        public Negation(INumeric node) : base(node) { }

        protected override RollProbabilityDistribution CreateProbabilityDistribution() =>
            _source.GetProbabilityDistribution()
                .Select(x => new Roll(-x.Outcome, x.Probability))
                .ToRollProbabilityDistribution();
    }
}
