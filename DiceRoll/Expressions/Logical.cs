using System.Runtime.InteropServices;

namespace DiceRoll.Expressions
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Logical
    {
        public readonly Binary Outcome;
        public readonly Probability Probability;
        
        public Logical(Binary outcome, Probability probability)
        {
            Outcome = outcome;
            Probability = probability;
        }
        
        public Logical(bool outcome, Probability probability) : this(new Binary(outcome), probability) { }
    }
}
