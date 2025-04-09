using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DiceRoll
{
    /// <summary>
    /// The exception that is thrown when an <see cref="IEnumerable{T}"/> parameter passed to a method
    /// points to an enumerable with no elements to enumerate.
    /// </summary>
    public sealed class EmptyEnumerableException : ArgumentException
    {
        public EmptyEnumerableException() { }
        public EmptyEnumerableException(string message) : base(message) { }
        public EmptyEnumerableException(string message, Exception innerException) : base(message, innerException) { }
        public EmptyEnumerableException(string message, string paramName) : base(message, paramName) { }

        public EmptyEnumerableException(string message, string paramName, Exception innerException) :
            base(message, paramName, innerException) { }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if <paramref name="enumerable"/> is null or
        /// <see cref="EmptyEnumerableException"/> if <paramref name="enumerable"/> has no elements to enumerate.
        /// </summary>
        /// <param name="enumerable">
        /// The enumerable to validate as non-null with at least 1 element to enumerate.
        /// </param>
        /// <param name="paramName">
        /// The name of the parameter with which <paramref name="enumerable"/> corresponds.
        /// If you omit this parameter, the name of <paramref name="enumerable"/> is used.
        /// </param>
        /// <typeparam name="T">The type parameter of <paramref name="enumerable"/>.</typeparam>
        /// <exception cref="ArgumentNullException">When <paramref name="enumerable"/> is null.</exception>
        /// <exception cref="EmptyEnumerableException">
        /// When <paramref name="enumerable"/> has no elements to enumerate.
        /// </exception>
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
