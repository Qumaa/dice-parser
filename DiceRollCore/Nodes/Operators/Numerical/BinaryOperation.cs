using System;

namespace DiceRoll
{
    public abstract class BinaryOperation : Operation
    {
        protected readonly INumeric _left;
        protected readonly INumeric _right;

        protected BinaryOperation(INumeric left, INumeric right)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);
            
            _left = left;
            _right = right;
        }
    }
}
