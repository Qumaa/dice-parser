using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class OperatorInvocationException : Exception
    {
        public OperatorInvocationException(string message) : base(message) { }
    }
}
