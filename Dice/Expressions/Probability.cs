namespace Dice.Expressions
{
    public readonly struct Probability
    {
        public static Probability Hundred => new(1d);
        public static Probability Zero => new(0d);
        
        public readonly double Value;

        public Probability(double value) 
        {
            Value = value;
        }

        public Probability Inversed() =>
            new(1d - Value);
    }
}
