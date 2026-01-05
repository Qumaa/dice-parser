using System;
using System.Globalization;

namespace DiceRoll
{
    // todo: use this to explicitly mark that this node is a parent to a distinct family of nodes
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
