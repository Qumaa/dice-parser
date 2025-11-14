using System;

namespace DiceRoll
{
    public sealed class Dice : Numeric
    {
        private readonly Random _random;
        public readonly int Faces;
        
        public Dice(Random random, int faces)
        {
            ArgumentNullException.ThrowIfNull(random);
            ArgumentOutOfRangeException.ThrowIfLessThan(faces, 1);

            _random = random;
            Faces = faces;
        }

        public override void NextEvaluation() =>
            CacheEvaluation(_random.Next(0, Faces) + 1);
        
        protected override RollProbabilityDistribution CreateProbabilityDistribution()
        {
            Roll[] rolls = new Roll[Faces];
            
            Probability eachOutcomeProbability = 1d / Faces;

            for (int i = 0; i < rolls.Length; i++)
                rolls[i] = new Roll(i + 1, eachOutcomeProbability);

            return rolls.ToRollProbabilityDistribution();
        }
    }
}
