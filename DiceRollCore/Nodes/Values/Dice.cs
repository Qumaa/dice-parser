using System;

namespace DiceRoll
{
    public sealed class Dice : Numeric
    {
        private readonly Random _random;
        private readonly int _faces;
        
        public Dice(Random random, int faces)
        {
            ArgumentNullException.ThrowIfNull(random);
            DiceFacesException.ThrowIfInvalid(faces);
            
            _random = random;
            _faces = faces;
        }

        protected override RollProbabilityDistribution CreateProbabilityDistribution()
        {
            Roll[] rolls = new Roll[_faces];
            
            Probability eachOutcomeProbability = new(1d / _faces);

            for (int i = 0; i < rolls.Length; i++)
                rolls[i] = new Roll(i + 1, eachOutcomeProbability);

            return rolls.ToRollProbabilityDistribution();
        }

        public override Outcome Evaluate() =>
            new(_random.Next(0, _faces) + 1);
    }
}
