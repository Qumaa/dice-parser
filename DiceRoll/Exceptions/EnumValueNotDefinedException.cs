using System;
using System.Runtime.CompilerServices;

namespace DiceRoll.Exceptions
{
    public static class EnumValueNotDefinedException
    {
        public static void ThrowIfValueNotDefined<TEnum>(TEnum value,
            [CallerArgumentExpression("value")] string paramName = null) 
            where TEnum : struct, Enum
        {
            if (!Enum.IsDefined(value))
                throw new EnumNotDefinedException<TEnum>(paramName, value);
        }
    }

    public sealed class EnumNotDefinedException<TEnum> : ArgumentOutOfRangeException where TEnum : struct, Enum
    {
        public EnumNotDefinedException() { }
            
        public EnumNotDefinedException(string paramName) : base(paramName) { }

        public EnumNotDefinedException(string message, Exception innerException) : base(message, innerException) { }

        public EnumNotDefinedException(TEnum value, Exception innerException) : this(
            GetErrorMessage(value), innerException) { }

        public EnumNotDefinedException(string paramName, TEnum actualValue) : base(paramName, actualValue, GetErrorMessage(actualValue)) { }
        public EnumNotDefinedException(string paramName, string message) : base(paramName, message) { }

        private static string GetErrorMessage(TEnum value) =>
            $"Value {Convert.ChangeType(value, value.GetTypeCode())} is not defined within the {nameof(TEnum)} enum.";
    }
}
