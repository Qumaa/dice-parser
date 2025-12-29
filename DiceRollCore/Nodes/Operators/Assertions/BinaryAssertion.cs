using System;

namespace DiceRoll
{
    public abstract class BinaryAssertion : Assertion
    {
        public readonly IAssertion Left;
        public readonly IAssertion Right;

        protected BinaryAssertion(IAssertion left, IAssertion right)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);
            
            Left = left;
            Right = right;
        }

        public override void NextEvaluation()
        {
            Left.NextEvaluation();
            Right.NextEvaluation();
        }
    }
}
