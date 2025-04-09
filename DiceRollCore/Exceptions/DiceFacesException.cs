using System;
using System.Runtime.CompilerServices;

namespace DiceRoll
{
    /// <summary>
    /// The exception that is thrown when the <see cref="DiceRoll.Dice(Random, int)"/> constructor receives 0 or
    /// a negative value.
    /// </summary>
    public sealed class DiceFacesException : ArgumentException
    {
        public DiceFacesException() { }
        public DiceFacesException(int facesNumber) : base(GetErrorMessage(facesNumber)) { }

        public DiceFacesException(int facesNumber, Exception innerException) : 
            base(GetErrorMessage(facesNumber), innerException) { }

        public DiceFacesException(int facesNumber, string paramName) : 
            base(GetErrorMessage(facesNumber), paramName) { }

        public DiceFacesException(int facesNumber, string paramName, Exception innerException) : 
            base(GetErrorMessage(facesNumber), paramName, innerException) { }

        /// <summary>
        /// Throws an <see cref="DiceFacesException"/> if <paramref name="facesNumber"/> is 0 or negative.
        /// </summary>
        /// <param name="facesNumber">The value to validate as above 0.</param>
        /// <param name="paramName">
        /// The name of the parameter with which <paramref name="facesNumber"/> corresponds.
        /// If you omit this parameter, the name of <paramref name="facesNumber"/> is used.
        /// </param>
        /// <exception cref="DiceFacesException">
        /// When <paramref name="facesNumber"/> is 0 or negative.
        /// </exception>
        public static void ThrowIfInvalid(int facesNumber,
            [CallerArgumentExpression("facesNumber")] string paramName = null)
        {
            if (facesNumber <= 0)
                throw new DiceFacesException(facesNumber, paramName);
        }
        private static string GetErrorMessage(int facesNumber) =>
            $"Invalid number of dice faces; expected at least 1, received {facesNumber}.";
    }
}
