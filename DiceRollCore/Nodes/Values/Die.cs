using System;

namespace DiceRoll
{
    public sealed class Die : Numeric
    {
        private static readonly Random _defaultRandom = new();
        public static readonly Die d4 = new(_defaultRandom, 4);
        public static readonly Die d6 = new(_defaultRandom, 6);
        public static readonly Die d8 = new(_defaultRandom, 8);
        public static readonly Die d10 = new(_defaultRandom, 10);
        public static readonly Die d12 = new(_defaultRandom, 12);
        public static readonly Die d20 = new(_defaultRandom, 20);
        public static readonly Die d100 = new(_defaultRandom, 100);
        
        private readonly Random _random;
        public readonly int Faces;
        
        public Die(Random random, int faces)
        {
            ArgumentNullException.ThrowIfNull(random);
            ArgumentOutOfRangeException.ThrowIfLessThan(faces, 1);

            _random = random;
            Faces = faces;
        }

        public override void NextEvaluation() =>
            CacheEvaluation(_random.Next(Faces) + 1);
        
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
