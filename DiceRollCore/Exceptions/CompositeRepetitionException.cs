using System;
using System.Runtime.CompilerServices;

namespace DiceRoll
{
    public sealed class CompositeRepetitionException : ArgumentException
    {
        public CompositeRepetitionException() { }
        public CompositeRepetitionException(string message) : base(message) { }
        public CompositeRepetitionException(string message, Exception innerException) : base(message, innerException) { }
        public CompositeRepetitionException(string message, string paramName) : base(message, paramName) { }
        public CompositeRepetitionException(string message, string paramName, Exception innerException) : base(message, paramName, innerException) { }
        
        public static int ThrowIfBelowTwo(int count, 
            [CallerArgumentExpression("count")] string paramName = null)
        {
            ArgumentNullException.ThrowIfNull(count, paramName);

            if (count < 2)
                throw new EmptyEnumerableException("Invalid repetition count; expected at least 2.", paramName);

            return count;
        }
    }
}
