using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DiceRoll.Input.Parsing
{
    internal static class CommonException
    {
        public static void ThrowIfParamsArrayIsEmpty<T>(T[] @params, [CallerArgumentExpression(nameof(@params))] string paramName = null)
        {
            if (@params is not { Length: > 0 })
                throw new ArgumentException($"Params array {paramName} is empty.", paramName);
        }

        public static void ThrowIfTypeIsNotNodeOrNull(Type type, [CallerArgumentExpression(nameof(type))] string paramName = null)
        {
            ArgumentNullException.ThrowIfNull(type);
            
            if (!typeof(INode).IsAssignableFrom(type))
                throw new ArgumentException(
                    $"{paramName} contains {type!.Name}, which is not an {nameof(INode)}-derived type."
                    );
        }

        public static void ThrowIfAnyTypeIsNotNode(IEnumerable<Type> types,
            [CallerArgumentExpression(nameof(types))] string paramName = null)
        {
            foreach (Type type in types)
                ThrowIfTypeIsNotNodeOrNull(type, paramName);
        }
    }
}
