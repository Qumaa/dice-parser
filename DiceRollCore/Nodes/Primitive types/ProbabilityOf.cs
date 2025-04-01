using System.Runtime.InteropServices;

namespace DiceRoll
{
    /// <summary>
    /// An arbitrary value of type <typeparamref name="T"/> wrapper that associates the value with certain
    /// <see cref="Probability">probability</see> of its occurrence among other values. 
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    [StructLayout(LayoutKind.Auto)]
    public readonly struct ProbabilityOf<T>
    {
        public readonly T Value;
        public readonly Probability Probability;
        
        /// <param name="value">The value.</param>
        /// <param name="probability">The probability of occurrence among other values.</param>
        public ProbabilityOf(T value, Probability probability)
        {
            Value = value;
            Probability = probability;
        }
    }
}
