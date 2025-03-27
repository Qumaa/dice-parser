using System;
using System.Runtime.CompilerServices;

namespace DiceRoll.Exceptions
{
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
