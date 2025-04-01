using System.Runtime.CompilerServices;

namespace DiceRoll
{
    /// <summary>
    /// The exception that is thrown when a not defined enum value is passed to a method.
    /// </summary>
    public static class EnumValueNotDefinedException
    {
        /// <summary>
        /// Throws an <see cref="EnumValueNotDefinedException{TEnum}"/> if <paramref name="value"/> is not defined
        /// within <typeparamref name="TEnum"/>.
        /// </summary>
        /// <param name="value">The value to validate as defined within <typeparamref name="TEnum"/>.</param>
        /// <param name="paramName">
        /// The name of the parameter with which <paramref name="value"/> corresponds.
        /// If you omit this parameter, the name of <paramref name="value"/> is used.
        /// </param>
        /// <typeparam name="TEnum">The <see cref="Enum"/> type value.</typeparam>
        /// <exception cref="EnumValueNotDefinedException{TEnum}">
        /// When <paramref name="value"/> is not defined within <typeparamref name="TEnum"/>.
        /// </exception>
        public static void ThrowIfValueNotDefined<TEnum>(TEnum value,
            [CallerArgumentExpression("value")] string paramName = null) 
            where TEnum : struct, Enum
        {
            if (!Enum.IsDefined(value))
                throw new EnumValueNotDefinedException<TEnum>(paramName, value);
        }
    }

    /// <inheritdoc cref="EnumValueNotDefinedException"/>
    /// <typeparam name="TEnum"><see cref="Enum"/> type.</typeparam>
    public sealed class EnumValueNotDefinedException<TEnum> : ArgumentOutOfRangeException where TEnum : struct, Enum
    {
        public EnumValueNotDefinedException() { }
            
        public EnumValueNotDefinedException(string paramName) : base(paramName) { }

        public EnumValueNotDefinedException(string message, Exception innerException) : base(message, innerException) { }

        public EnumValueNotDefinedException(TEnum value, Exception innerException) : this(
            GetErrorMessage(value), innerException) { }

        public EnumValueNotDefinedException(string paramName, TEnum actualValue) : base(paramName, actualValue, GetErrorMessage(actualValue)) { }
        public EnumValueNotDefinedException(string paramName, string message) : base(paramName, message) { }

        private static string GetErrorMessage(TEnum value) =>
            $"Value {Convert.ChangeType(value, value.GetTypeCode())} is not defined within the {nameof(TEnum)} enum.";
    }
}
