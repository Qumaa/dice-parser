using System;

namespace Dice.Expressions
{
    public class Dice : IAnalyzable
    {
        private readonly Random _random;
        private readonly int _faces;
        private readonly int _number;

        public ProbabilityDistribution GetProbabilityData() =>
            ProbabilityDistribution.OfDice(_faces, _number);

        public Dice(Random random, int faces, int number)
        {
            _random = random;
            _faces = faces;
            _number = number;
        }

        public Outcome Evaluate() =>
            new(_random.Next(_number, _faces * _number));
    }
}
