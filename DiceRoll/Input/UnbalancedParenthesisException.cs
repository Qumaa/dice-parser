using System;

namespace DiceRoll.Input
{
    internal sealed class UnbalancedParenthesisException : ArithmeticException
    {
        public UnbalancedParenthesisException() { }
        public UnbalancedParenthesisException(string message) : base(message) { }
        public UnbalancedParenthesisException(string message, Exception innerException) : base(message, innerException) { }
    }
}
