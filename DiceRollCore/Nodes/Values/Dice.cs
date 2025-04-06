using System;
using System.Linq;

namespace DiceRoll
{
    /// <summary>
    /// A <see cref="INumeric">numerical node</see> that represents a singular N-sided die.
    /// Produces random results from 1 to number specified in the <see cref="Dice(Random, int)">constructor</see>.
    /// </summary>
    public sealed class Dice : NumericNode
    {
        private readonly Random _random;
        private readonly int _faces;
        
        /// <param name="random">A random numbers generator instance.</param>
        /// <param name="faces">Maximum possible random value (inclusive). Must be at least 1.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="random"/> is null.</exception>
        /// <exception cref="DiceFacesException">When <paramref name="faces"/> is 0 or less.</exception>
        public Dice(Random random, int faces)
        {
            ArgumentNullException.ThrowIfNull(random);
            DiceFacesException.ThrowIfInvalid(faces);
            
            _random = random;
            _faces = faces;
        }

        public override RollProbabilityDistribution GetProbabilityDistribution()
        {
            Probability eachOutcomeProbability = new(1d / _faces);

            RollProbabilityDistribution distribution = new(Enumerable.Range(1, _faces)
                .Select(faceValue => new Outcome(faceValue))
                .Select(outcome => new Roll(outcome, eachOutcomeProbability)));

            return distribution;
        }

        public override Outcome Evaluate() =>
            new(_random.Next(0, _faces) + 1);
    }
}
