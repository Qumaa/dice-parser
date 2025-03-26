using System;
using System.Linq;
using DiceRoll.Exceptions;

namespace DiceRoll.Nodes
{
    public sealed class Dice : IAnalyzable
    {
        private readonly Random _random;
        private readonly int _faces;

        public Dice(Random random, int faces)
        {
            ArgumentNullException.ThrowIfNull(random);
            FacesNumberException.ThrowIfInvalid(faces);
            
            _random = random;
            _faces = faces;
        }

        public RollProbabilityDistribution GetProbabilityDistribution()
        {
            Probability eachFaceProbability = new(1d / _faces);

            RollProbabilityDistribution distribution = new(Enumerable.Range(1, _faces)
                .Select(faceValue => new Outcome(faceValue))
                .Select(outcome => new Roll(outcome, eachFaceProbability)));

            return distribution;
        }

        public Outcome Evaluate() =>
            new(_random.Next(1, _faces));
    }
}
