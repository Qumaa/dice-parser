using System;

namespace DiceRoll.Input
{
    public sealed class OperatorInvocationException : Exception
    {
        public OperatorInvocationException(string message) : base(message) { }
    }
}
