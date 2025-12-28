using System;

namespace DiceRoll
{
    public abstract class BinaryTransformation : Numeric
    {
        public readonly INumeric Left;
        public readonly INumeric Right;

        protected BinaryTransformation(INumeric left, INumeric right)
        {
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
