using System;

namespace DiceRoll.Nodes
{
    public abstract class MergeTransformation : Transformation
    {
        protected readonly IAnalyzable _other;

        protected MergeTransformation(IAnalyzable source, IAnalyzable other) : base(source)
        {
            ArgumentNullException.ThrowIfNull(other);
            
            _other = other;
        }
    }
}
