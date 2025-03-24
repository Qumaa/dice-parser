using System.Runtime.InteropServices;

namespace Dice.Expressions
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct OutcomeProbability
    {
        public readonly Outcome Outcome;
        public readonly Probability Probability;
        
        public OutcomeProbability(Outcome outcome, Probability probability)
        {
            Outcome = outcome;
            Probability = probability;
        }

        public OutcomeProbability(int rollResult, double probability) : 
            this(new Outcome(rollResult), new Probability(probability)) 
        { }

        public OutcomeProbability(int rollResult, Probability probability) : this(new Outcome(rollResult), probability) { }
    }
}
