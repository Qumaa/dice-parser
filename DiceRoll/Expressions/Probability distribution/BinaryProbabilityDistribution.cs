using System.Collections.Generic;

namespace DiceRoll.Expressions
{
    public sealed class BinaryProbabilityDistribution : ProbabilityDistribution<Binary>
    {
        public BinaryProbabilityDistribution(Probability ofTrue) : 
            base(ToEnumerable(ofTrue)) { }

        private static IEnumerable<Binary> ToEnumerable(Probability ofTrue)
        {
            yield return new Binary(true, ofTrue);
            yield return new Binary(false, ofTrue.Inversed());
        }
    }
}
