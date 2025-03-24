namespace Dice.Expressions
{
    public class Constant : IAnalyzable
    {
        private readonly int _value;

        public Constant(int value) 
        {
            _value = value;
        }

        public Outcome Evaluate() =>
            new(_value);

        public ProbabilityDistribution GetProbabilityData() =>
            ProbabilityDistribution.OfConstant(_value);
    }
}
