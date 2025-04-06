using System.Linq;

namespace DiceRoll
{
    public sealed class Negation : Transformation
    {
        public Negation(INumeric node) : base(node) { }
        
        public override Outcome Evaluate() =>
            -_source.Evaluate();

        public override RollProbabilityDistribution GetProbabilityDistribution() =>
            _source.GetProbabilityDistribution()
                .Select(static x => new Roll(-x.Outcome, x.Probability))
                .ToRollProbabilityDistribution();
    }
}
