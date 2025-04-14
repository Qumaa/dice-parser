using System;
using System.Linq;

namespace DiceRoll.Input
{
    public static class ParsingErrorMessages
    {
        public const string UNUSED_OPERAND = "This operand doesn't take part in the expression.";
        public const string TRAILING_DELAYED_OPERATOR = "This operator didn't receive enough right-side operands.";
        public const string UNBALANCED_PARENTHESIS =
            "Couldn't close parenthesis: unbalanced parenthesis within the expression.";
        public const string PUSHING_PREMATURELY =
            "An attempt to return the result of working on fewer operands than the declared arity was intercepted.";

        public static string ExceedingArity(int arity)
        {
            const string format =
                "An attempt to work on more operands than the arity of the operator was intercepted. The operator's arity ({0}) is faulty.";

            return string.Format(format, arity.ToString());
        }

        public static string OperandsExpected(int operandsExpected, int operandsReceived)
        {
            const string format = "Couldn't invoke operator: expected {0} operands, received {1}.";
            
            return string.Format(format, operandsExpected.ToString(), operandsReceived.ToString());
        }
        
        public static string UnknownToken(in Substring tokenMatch)
        {
            const string format = "Couldn't parse the following unknown token: \"{0}\".";
            
            return string.Format(format, tokenMatch.ToString());
        }
        
        public static string OperandTypeMismatch(Type expectedOperandType, Type receivedOperandType)
        {
            const string format = "Couldn't invoke operator: the operand evaluates to {0} (expected {1}).";
            
            return string.Format(format, _FormatTypeName(receivedOperandType), _FormatTypeName(expectedOperandType));
            
            string _FormatTypeName(Type type) =>
                type.IsInterface ? _FormatInterfaceName(type) : _FormatInterfaceImplementationName(type);

            string _FormatInterfaceName(Type interfaceType) =>
                _GetNodeEvaluationType(interfaceType.GetInterfaces()).Name;

            string _FormatInterfaceImplementationName(Type implementationType) =>
                _FormatInterfaceName(implementationType.GetInterfaces()
                    .First(i => new[] { typeof(INumeric), typeof(IOperation), typeof(IAssertion) }.Any(x => i == x)));

            Type _GetNodeEvaluationType(Type[] interfaces) =>
                interfaces.First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INode<>))
                    .GetGenericArguments()[0];
        }
    }
}
