using System.Runtime.InteropServices;

namespace DiceRoll
{
    /// <summary>
    /// <para>Represents the numerical result of a node named after a dice roll.</para>
    /// <para>
    /// A shorthand for <see cref="ProbabilityOf{T}">ProbabilityOf</see> of type <see cref="Outcome"/>
    /// with a reasonable naming.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Roll
    {
        public readonly Outcome Outcome;
        public readonly Probability Probability;
        
        /// <param name="outcome">The outcome.</param>
        /// <param name="probability">The probability of occurrence among other outcomes.</param>
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
