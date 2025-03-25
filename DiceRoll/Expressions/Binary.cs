namespace DiceRoll.Expressions
{
    public readonly struct Binary
    {
        public readonly bool Value;
        public readonly Probability Probability;
        
        public Binary(bool value, Probability probability)
        {
            Value = value;
            Probability = probability;
        }
    }
}
