using System;
using System.Linq;

namespace DiceRoll.Input
{
    public sealed class OperatorInvocationException : Exception
    {
        private const string _MESSAGE_EXPECTED_OPERANDS = "Couldn't invoke operator: expected {0} operands, received {1}.";
        private const string _MESSAGE_TYPE_MISMATCH = "Couldn't invoke operator: the operand evaluates to {0} (expected {1}).";

        private static readonly Type[] _types = { typeof(INumeric), typeof(IOperation), typeof(IConditional) };

        public OperatorInvocationException(Type expectedOperandType, Type actualOperandType) : 
            base(FormatMessage(expectedOperandType, actualOperandType)) { }

        public OperatorInvocationException(int operandsExpected, int operandsReceived) : 
            base(FormatMessage(operandsExpected, operandsReceived)) { }

        private static string FormatMessage(Type expectedOperandType, Type actualOperandType) =>
            string.Format(_MESSAGE_TYPE_MISMATCH, FormatTypeName(actualOperandType), FormatTypeName(expectedOperandType));

        private static string FormatMessage(int operandsExpected, int operandsReceived) =>
            string.Format(_MESSAGE_EXPECTED_OPERANDS, operandsExpected.ToString(), operandsReceived.ToString());

        private static string FormatTypeName(Type type) =>
            type.IsInterface ? FormatInterfaceName(type) : FormatInterfaceImplementationName(type);

        private static string FormatInterfaceName(Type interfaceType) =>
            GetNodeEvaluationType(interfaceType.GetInterfaces()).Name;

        private static string FormatInterfaceImplementationName(Type implementationType) =>
            FormatInterfaceName(implementationType.GetInterfaces()
                .First(static i => _types.Any(x => i == x)));

        private static Type GetNodeEvaluationType(Type[] interfaces) =>
            interfaces.First(static i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INode<>))
                .GetGenericArguments()[0];
    }
}
