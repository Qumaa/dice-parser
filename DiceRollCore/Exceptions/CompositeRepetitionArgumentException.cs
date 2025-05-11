using System;
using System.Runtime.CompilerServices;

namespace DiceRoll
{
    public sealed class CompositeRepetitionArgumentException : ArgumentException
    {
        public CompositeRepetitionArgumentException() { }
        public CompositeRepetitionArgumentException(string message) : base(message) { }

        public CompositeRepetitionArgumentException(string message, Exception innerException) :
            base(message, innerException) { }

        public CompositeRepetitionArgumentException(string message, string paramName) : base(message, paramName) { }

        public CompositeRepetitionArgumentException(string message, string paramName, Exception innerException) :
            base(message, paramName, innerException) { }

        public static int ThrowIfBelowOne(int count,
            [CallerArgumentExpression("count")] string paramName = null)
        {
            if (count < 1)
                throw new CompositeRepetitionArgumentException("Invalid composition repetition parameter; expected at least 1.", paramName);

            return count;
        }
    }
}
