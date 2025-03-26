using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DiceRoll.Exceptions
{
    public sealed class EmptySequenceException : ArgumentException
    {
        public EmptySequenceException() { }
        public EmptySequenceException(string message) : base(message) { }
        public EmptySequenceException(string message, Exception innerException) : base(message, innerException) { }
        public EmptySequenceException(string message, string paramName) : base(message, paramName) { }
        public EmptySequenceException(string message, string paramName, Exception innerException) : base(message, paramName, innerException) { }

        public static void ThrowIfNullOrEmpty<T>(IEnumerable<T> sequence, 
            [CallerArgumentExpression("sequence")] string paramName = null)
        {
            ArgumentNullException.ThrowIfNull(sequence, paramName);

            if (!sequence.TryGetNonEnumeratedCount(out int count))
                count = sequence.Count();

            if (count is 0)
                throw new EmptySequenceException("Invalid sequence; expected at least 1 element.", paramName);
        }
    }
}
