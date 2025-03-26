using System.Runtime.InteropServices;

namespace DiceRoll.Nodes
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

        public Roll(int outcome, double probability) : 
            this(new Outcome(outcome), new Probability(probability)) 
        { }

        public Roll(int outcome, Probability probability) : this(new Outcome(outcome), probability) { }
    }
}
