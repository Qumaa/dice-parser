using System;
using System.Globalization;

namespace DiceRoll
{
    [AttributeUsage(AttributeTargets.Interface)]
    public sealed class BaseTypeAttribute : Attribute
    {
        public readonly string ReadableName;
        
        public BaseTypeAttribute(string readableName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(readableName);
            
            ReadableName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(readableName);
        }
    }
}
