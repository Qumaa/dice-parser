using System;
using System.Data;
using System.Runtime.CompilerServices;

namespace DiceRoll.Input.Parsing
{
    public sealed class ConstructorException : Exception
    {
        public ConstructorException(string message) : base(message) { }

        public static void ThrowIfParamsArrayIsEmpty<T>(T[] @params, [CallerArgumentExpression(nameof(@params))] string paramName = null)
        {
            if (@params is not { Length: > 0 })
                throw new ConstructorException($"Params array {paramName} is empty.");
        }
    }
}
