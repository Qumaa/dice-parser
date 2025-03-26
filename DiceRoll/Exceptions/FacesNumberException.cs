using System;
using System.Runtime.CompilerServices;

namespace DiceRoll.Exceptions
{
    public sealed class FacesNumberException : ArgumentException
    {
        public FacesNumberException() { }
        public FacesNumberException(int facesNumber) : base(GetErrorMessage(facesNumber)) { }

        public FacesNumberException(int facesNumber, Exception innerException) : base(
            GetErrorMessage(facesNumber), innerException) { }

        public FacesNumberException(int facesNumber, string paramName) : base(GetErrorMessage(facesNumber),
            paramName) { }

        public FacesNumberException(int facesNumber, string paramName, Exception innerException) : base(
            GetErrorMessage(facesNumber), paramName, innerException) { }

        public static void ThrowIfInvalid(int facesNumber,
            [CallerArgumentExpression("facesNumber")] string paramName = null)
        {
            if (facesNumber <= 0)
                throw new FacesNumberException(facesNumber, paramName);
        }
        private static string GetErrorMessage(int facesNumber) =>
            $"Invalid number of dice faces; expected at least 1, received {facesNumber}.";
    }
}
