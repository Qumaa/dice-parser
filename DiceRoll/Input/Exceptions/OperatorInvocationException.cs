using System;

namespace DiceRoll.Input
{
    public sealed class OperatorInvocationException : Exception
    {
        private const string _MESSAGE = "Couldn't invoke operator: expected {0} operands, received {1}.";

        public OperatorInvocationException(int operandsExpected, int operandsReceived) : 
            base(FormatMessage(operandsExpected, operandsReceived)) { }

        private static string FormatMessage(int operandsExpected, int operandsReceived) =>
            string.Format(_MESSAGE, operandsExpected.ToString(), operandsReceived.ToString());
    }
}
