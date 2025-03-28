using System.Collections.Generic;

namespace DiceRoll.Nodes
{
    /// <summary>
    /// A boolean implementation of <see cref="ProbabilityDistribution{T}">ProbabilityDistribution</see>
    /// of type <see cref="Logical"/> with a reasonable naming.
    /// Always contains 2 boolean values.
    /// </summary>
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
