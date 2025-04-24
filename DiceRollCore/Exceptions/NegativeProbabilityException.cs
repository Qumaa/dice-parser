using System;

namespace DiceRoll
{
    public sealed class NegativeProbabilityException : ArgumentException
    {
        public NegativeProbabilityException() { }
        public NegativeProbabilityException(double probability) : base(GetErrorMessage(probability)) { }

        public NegativeProbabilityException(double probability, Exception innerException) :
            base(GetErrorMessage(probability), innerException) { }

        public NegativeProbabilityException(double probability, string paramName) :
            base(GetErrorMessage(probability), paramName) { }

        public NegativeProbabilityException(double probability, string paramName, Exception innerException) :
            base(GetErrorMessage(probability), paramName, innerException) { }

        private static string GetErrorMessage(double probability) =>
            $"Invalid probability value; expected 0 or larger, received {probability}.";
    }
}
