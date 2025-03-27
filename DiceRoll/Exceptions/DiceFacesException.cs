using System;
using System.Runtime.CompilerServices;

namespace DiceRoll.Exceptions
{
    public sealed class DiceFacesException : ArgumentException
    {
        public DiceFacesException() { }
        public DiceFacesException(int facesNumber) : base(GetErrorMessage(facesNumber)) { }

        public DiceFacesException(int facesNumber, Exception innerException) : base(
            GetErrorMessage(facesNumber), innerException) { }

        public DiceFacesException(int facesNumber, string paramName) : base(GetErrorMessage(facesNumber),
            paramName) { }

        public DiceFacesException(int facesNumber, string paramName, Exception innerException) : base(
            GetErrorMessage(facesNumber), paramName, innerException) { }

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
