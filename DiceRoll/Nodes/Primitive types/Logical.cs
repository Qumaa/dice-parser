using System.Runtime.InteropServices;

namespace DiceRoll.Nodes
{
    /// <summary>
    /// <para>Represent the boolean outcome of a node.</para>
    /// <para>A shorthand for <see cref="ProbabilityOf{T}">ProbabilityOf</see> of type <see cref="Binary"/></para>
    /// with a reasonable naming.
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Logical
    {
        public readonly Binary Outcome;
        public readonly Probability Probability;
        
        /// <param name="outcome">The outcome.</param>
        /// <param name="probability">The probability of occurrence compared to the opposite outcome.</param>
        public Logical(Binary outcome, Probability probability)
        {
            Outcome = outcome;
            Probability = probability;
        }
        
        public Logical(bool outcome, Probability probability) : this(new Binary(outcome), probability) { }
    }
}
