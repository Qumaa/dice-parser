using System;

namespace DiceRoll
{
    public abstract class Transformation : Numeric
    {
        public readonly INumeric Source;
        
        protected Transformation(INumeric source)
        {
            ArgumentNullException.ThrowIfNull(source);
            
            Source = source;
        }
        
        protected abstract override RollProbabilityDistribution CreateProbabilityDistribution();

        public override void NextEvaluation() =>
            Source.NextEvaluation();
    }
}
