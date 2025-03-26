using System.Collections.Generic;

namespace DiceRoll.Nodes
{
    public sealed class LogicalProbabilityDistribution : ProbabilityDistribution<Logical>
    {
        public LogicalProbabilityDistribution(Probability ofTrue) : 
            base(ToEnumerable(ofTrue)) { }

        private static IEnumerable<Logical> ToEnumerable(Probability ofTrue)
        {
            yield return new Logical(true, ofTrue);
            yield return new Logical(false, ofTrue.Inversed());
        }
    }
}
