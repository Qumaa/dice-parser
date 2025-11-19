using System;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    internal sealed class OperatorInvocationException : Exception
    {
        public OperatorInvocationException(string message) : base(message) { }

        public static OperatorInvocationException InvalidOperandCast(int operandIndex,
            Signature operatorSignature, Type requestedType)
        {
            string signature = $"<{string.Join(',', operatorSignature.EnumerateOperandTypes().Select(x => x.Name))}>";

            string message = 
                $"This operator defines a {signature} signature where operand number {operandIndex} type is {operatorSignature.GetOperandTypes()[operandIndex].Name}, but {requestedType} was asked instead. Invocation behaviour is flawed.";
            
            return new OperatorInvocationException(message);
        }

        public static OperatorInvocationException NoMatchingSignature(OperatorInvocationBehaviour invocationBehaviour,
            in Substring operatorSubstring)
        {
            string operatorString = operatorSubstring.ToString();

            string expectedSignatures = string.Join(
                ',',
                invocationBehaviour.Invokers
                    .Select(invoker => _SignatureToString(
                            invoker.Signature.EnumerateOperandTypes().Select(type => type.Name).ToArray(),
                            invoker.LeftArity,
                            operatorString
                            )
                        )
                );
            
            string message =
                $"None of this operator's defined signatures could handle passed arguments. Candidates are {expectedSignatures}.";
            
            return new OperatorInvocationException(message);
            
            static string _SignatureToString(string[] argumentTypeNames, int operatorPosition, string operatorString)
            {
                string leftArguments = string.Join(
                    ',',
                    argumentTypeNames.Take(operatorPosition)
                    );
                string rightArguments = string.Join(
                    ',',
                    argumentTypeNames.Skip(operatorPosition)
                    );

                return $"<{leftArguments} {operatorString} {rightArguments}>";
            }
        }

        public static OperatorInvocationException NotEnoughOperands(int arity, int operandsCount) =>
            new($"This operator expected {arity} operand(-s), but received {operandsCount}.");
    }
}
