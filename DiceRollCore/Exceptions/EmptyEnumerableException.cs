using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DiceRoll
{
    public sealed class EmptyEnumerableException : ArgumentException
    {
        public EmptyEnumerableException() { }
        public EmptyEnumerableException(string message) : base(message) { }
        public EmptyEnumerableException(string message, Exception innerException) : base(message, innerException) { }
        public EmptyEnumerableException(string message, string paramName) : base(message, paramName) { }

        public EmptyEnumerableException(string message, string paramName, Exception innerException) :
            base(message, paramName, innerException) { }

        public static void ThrowIfNullOrEmpty<T>(IEnumerable<T> enumerable,
            [CallerArgumentExpression("enumerable")] string paramName = null
        )
        {
            ArgumentNullException.ThrowIfNull(enumerable, paramName);

            if (!ContainsAtLeastOneElement(enumerable))
                throw new EmptyEnumerableException("Invalid enumerable; expected at least 1 element.", paramName);
        }

        private static bool ContainsAtLeastOneElement<T>(IEnumerable<T> enumerable)
        {
            if (enumerable.TryGetNonEnumeratedCount(out int count) && count is not 0)
                return true;

            using IEnumerator<T> enumerator = enumerable.GetEnumerator();
            return enumerator.MoveNext();
        }
    }
}
