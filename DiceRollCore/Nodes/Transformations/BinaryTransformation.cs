using System;

namespace DiceRoll
{
    public abstract class BinaryTransformation : Numeric
    {
        protected readonly INumeric _left;
        protected readonly INumeric _right;

        protected BinaryTransformation(INumeric left, INumeric right)
        {
            ArgumentNullException.ThrowIfNull(right);

            _left = left;
            _right = right;
        }

        public override void Next()
        {
            _left.Next();
            _right.Next();
        }
    }
}
