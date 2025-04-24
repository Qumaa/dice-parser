using System;
using System.Runtime.CompilerServices;

namespace DiceRoll
{
    public static class EnumValueNotDefinedException
    {
        public static void ThrowIfValueNotDefined<TEnum>(
            TEnum value,
            [CallerArgumentExpression("value")] string paramName = null
        )
            where TEnum : struct, Enum
        {
            if (!Enum.IsDefined(value))
                throw new EnumValueNotDefinedException<TEnum>(paramName, value);
        }
    }

    public sealed class EnumValueNotDefinedException<TEnum> : ArgumentOutOfRangeException where TEnum : struct, Enum
    {
        public EnumValueNotDefinedException() { }

        public EnumValueNotDefinedException(string paramName) : base(paramName) { }

        public EnumValueNotDefinedException(string message, Exception innerException) :
            base(message, innerException) { }

        public EnumValueNotDefinedException(TEnum value, Exception innerException) :
            this(GetErrorMessage(value), innerException) { }

        public EnumValueNotDefinedException(string paramName, TEnum actualValue) :
            base(paramName, actualValue, GetErrorMessage(actualValue)) { }

        public EnumValueNotDefinedException(string paramName, string message) : base(paramName, message) { }

        private static string GetErrorMessage(TEnum value) =>
            $"Value {Convert.ChangeType(value, value.GetTypeCode())} is not defined within the {nameof(TEnum)} enum.";
    }
}
