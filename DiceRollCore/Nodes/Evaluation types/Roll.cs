using System.Runtime.InteropServices;

namespace DiceRoll
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Roll
    {
        public readonly Outcome Outcome;
        public readonly Probability Probability;
        
        public Roll(Outcome outcome, Probability probability)
        {
            Outcome = outcome;
            Probability = probability;
        }
    }
}
