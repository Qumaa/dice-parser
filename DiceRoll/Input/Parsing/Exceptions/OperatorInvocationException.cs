using System;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    public sealed class OperatorInvocationException : Exception
    {
        public OperatorInvocationException(string message) : base(message) { }

        public static OperatorInvocationException BadSignature(BadSignatureException e,
            in OperatorInvocationBehaviour invocationBehaviour, in Substring operatorSubstring)
        {
            const char separator = ',';
            
            string leftArguments = string.Join(separator, e.ExpectedTypes.Take(invocationBehaviour.LeftArity).Select(x => x.Name));
            string rightArguments = string.Join(separator, e.ExpectedTypes.Skip(invocationBehaviour.LeftArity).Select(x => x.Name));
            
            string message =
                $"This operator has bad signature; <{leftArguments} {operatorSubstring.ToString()} {rightArguments}> is expected.";
            
            return new OperatorInvocationException(message);
        }
    }
}
