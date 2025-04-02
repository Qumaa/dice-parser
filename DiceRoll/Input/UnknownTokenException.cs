using System;

namespace DiceRoll.Input
{
    public class UnknownTokenException : ArithmeticException
    {
        public UnknownTokenException() { }
        public UnknownTokenException(string message) : base(message) { }
        public UnknownTokenException(string message, Exception innerException) : base(message, innerException) { }
    }
}
