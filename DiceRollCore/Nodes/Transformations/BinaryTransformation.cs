using System;

namespace DiceRoll
{
    public abstract class BinaryTransformation : Transformation
    {
        protected readonly INumeric _other;

        protected BinaryTransformation(INumeric source, INumeric other) : base(source)
        {
            ArgumentNullException.ThrowIfNull(other);
            
            _other = other;
        }
    }
}
