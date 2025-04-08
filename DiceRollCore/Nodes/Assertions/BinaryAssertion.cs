using System;

namespace DiceRoll
{
    public abstract class BinaryAssertion : Assertion
    {
        protected readonly IAssertion _left;
        protected readonly IAssertion _right;

        protected BinaryAssertion(IAssertion left, IAssertion right)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);
            
            _left = left;
            _right = right;
        }
    }
}
