using System;
using System.Runtime.CompilerServices;

namespace DiceRoll.Exceptions
{
    /// <summary>
    /// The exception that is thrown when the <see cref="DiceRoll.Nodes.Probability(double)"/> constructor receives
    /// a negative value.
    /// </summary>
    public sealed class NegativeProbabilityException : ArgumentException
    {
        public NegativeProbabilityException() { }
        public NegativeProbabilityException(double probability) : base(GetErrorMessage(probability)) { }

        public NegativeProbabilityException(double probability, Exception innerException) : base(
            GetErrorMessage(probability), innerException) { }

        public NegativeProbabilityException(double probability, string paramName) : base(GetErrorMessage(probability),
            paramName) { }

        public NegativeProbabilityException(double probability, string paramName, Exception innerException) : base(
            GetErrorMessage(probability), paramName, innerException) { }

        /// <summary>
        /// Throws a <see cref="NegativeProbabilityException"/> if <paramref name="probability"/> is negative.
        /// </summary>
        /// <param name="probability">The value to validate as non-negative.</param>
        /// <param name="paramName">
        /// The name of the parameter with which <paramref name="probability"/> corresponds.
        /// If you omit this parameter, the name of <paramref name="probability"/> is used.
        /// </param>
        /// <exception cref="NegativeProbabilityException">
        /// When if <paramref name="probability"/> is negative.
        /// </exception>
        public static void ThrowIfNegative(double probability,
            [CallerArgumentExpression("probability")] string paramName = null)
        {
            if (probability <= 0)
                throw new NegativeProbabilityException(probability, paramName);
        }
        private static string GetErrorMessage(double probability) =>
            $"Invalid probability value; expected 0 or larger, received {probability}.";
    }
}
