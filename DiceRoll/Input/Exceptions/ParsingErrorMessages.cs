using System;
using System.Linq;

namespace DiceRoll.Input
{
    public static class ParsingErrorMessages
    {
        public const string UNUSED_OPERAND = "This operand doesn't take part in the expression.";
        public const string TRAILING_DELAYED_OPERATOR = "This operator didn't receive enough right-side operands.";
        public const string UNBALANCED_PARENTHESIS = "This parenthesis has no matching opening pair.";
        public const string PUSHING_PREMATURELY =
            "This operator returned a result before consuming all required operands.";

        public static string ExceedingArity(int arity)
        {
            const string format = "This operator attempted to consume more operands than its declared arity ({0}).";

            return string.Format(format, arity.ToString());
        }

        public static string OperandsExpected(int operandsExpected, int operandsReceived)
        {
            const string format = "This operator expected {0} operand(-s), but received {1}.";
            
            return string.Format(format, operandsExpected.ToString(), operandsReceived.ToString());
        }
        
        public static string UnknownToken(in Substring tokenMatch)
        {
            const string format = "This token is not recognized (\"{0}\").";
            
            return string.Format(format, tokenMatch.ToString());
        }
        
        public static string OperandTypeMismatch(Type expectedOperandType, Type receivedOperandType)
        {
            const string format = "This operator's operand evaluates to {0}, but {1} was expected.";
            
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
