using System;
using System.Linq;

namespace DiceRoll.Expressions
{
    public sealed class Dice : IAnalyzable
    {
        private readonly Random _random;
        private readonly int _faces;

        public RollProbabilityDistribution GetProbabilityDistribution()
        {
            Probability eachFaceProbability = new(1d / _faces);

            RollProbabilityDistribution distribution = new(Enumerable.Range(1, _faces)
                .Select(faceValue => new Outcome(faceValue))
                .Select(outcome => new Roll(outcome, eachFaceProbability)));

            return distribution;
        }

        public Dice(Random random, int faces)
        {
            _random = random;
            _faces = faces;
        }

        public Outcome Evaluate() =>
            new(_random.Next(1, _faces));
    }
}
