using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DiceRoll.Input.Parsing
{
    internal sealed class ConstructorException : Exception
    {
        public ConstructorException(string message) : base(message) { }

        public static void ThrowIfParamsArrayIsEmpty<T>(T[] @params, [CallerArgumentExpression(nameof(@params))] string paramName = null)
        {
            if (@params is not { Length: > 0 })
                throw new ConstructorException($"Params array {paramName} is empty.");
        }

        public static void ThrowIfBelowZero(int value, [CallerArgumentExpression(nameof(value))] string paramName = null)
        {
            if (value < 0)
                throw new ConstructorException($"{paramName} cannot be lower than 0.");
        }

        public static void ThrowIfTypeIsNotNode(Type type, [CallerArgumentExpression(nameof(type))] string paramName = null)
        {
            if (!typeof(INode).IsAssignableFrom(type))
                throw new ConstructorException(
                    $"{paramName} contains {type.Name}, which is not a {nameof(INode)}-derived type."
                    );
        }

        public static void ThrowIfAnyTypeIsNotNode(IEnumerable<Type> types,
            [CallerArgumentExpression(nameof(types))] string paramName = null)
        {
            foreach (Type type in types)
                ThrowIfTypeIsNotNode(type, paramName);
        }
    }
}
