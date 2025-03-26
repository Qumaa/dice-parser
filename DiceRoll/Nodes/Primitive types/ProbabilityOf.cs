using System.Runtime.InteropServices;

namespace DiceRoll.Nodes
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct ProbabilityOf<T>
    {
        public readonly T Value;
        public readonly Probability Probability;
        
        public ProbabilityOf(T value, Probability probability)
        {
            Value = value;
            Probability = probability;
        }
    }
}
