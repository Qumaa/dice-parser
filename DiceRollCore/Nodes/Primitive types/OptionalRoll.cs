using System.Runtime.InteropServices;

namespace DiceRoll
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct OptionalRoll
    {
        public readonly Optional<Outcome> Outcome;
        public readonly Probability Probability;
        
        public OptionalRoll(Outcome outcome, Probability probability)
        {
            Outcome = new Optional<Outcome>(outcome);
            Probability = probability;
        }
        
        public OptionalRoll(Probability probability)
        {
            Outcome = Optional<Outcome>.Empty;
            Probability = probability;
        }
    }
}
