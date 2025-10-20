using System;

namespace DiceRoll
{
    public abstract class Transformation : Numeric
    {
        protected readonly INumeric _source;
        
        protected Transformation(INumeric source)
        {
            ArgumentNullException.ThrowIfNull(source);
            
            _source = source;
        }
        
        protected abstract override RollProbabilityDistribution CreateProbabilityDistribution();

        public override void NextEvaluation() =>
            _source.NextEvaluation();
    }
}
