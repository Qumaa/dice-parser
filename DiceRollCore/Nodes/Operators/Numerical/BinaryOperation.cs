using System;

namespace DiceRoll
{
    public abstract class BinaryOperation : Operation
    {
        public readonly INumeric Left;
        public readonly INumeric Right;

        protected BinaryOperation(INumeric left, INumeric right)
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
