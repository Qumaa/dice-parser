using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
                $"This operator defines a {signature} signature where operand number {operandIndex + 1} type is {operatorSignature.OperandTypes[operandIndex].Name}, but {requestedType} was asked instead. Invocation behaviour is flawed.";
            
            return new OperatorInvocationException(message);
        }

        public static OperatorInvocationException NoMatchingSignature(IEnumerable<OperatorInvocationBehaviour> invocationBehaviours,
            string operatorString)
        {
            string expectedSignatures = string.Join(
                ",\n",
                invocationBehaviours
                    .SelectMany(x => x.Invokers)
                    .Select(invoker => _SignatureToString(
                            invoker.Signature.EnumerateOperandTypes().Select(_GetNodeTypeName).ToArray(),
                            invoker.Arity.Left,
                            operatorString
                            )
                        )
                );
            
            string message =
                $"None of this operator's defined signatures could handle passed arguments. Candidates are:\n{expectedSignatures}";
            
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

            static string _GetNodeTypeName(Type type)
            {
                BaseTypeAttribute attribute = type.GetCustomAttribute<BaseTypeAttribute>(inherit: true);

                return attribute is null ?
                    type.Name :
                    attribute.ReadableName;
            }
        }
    }
}
