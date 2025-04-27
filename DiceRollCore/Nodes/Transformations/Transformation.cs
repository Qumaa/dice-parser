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

        protected abstract override Outcome GetNextEvaluation();

        protected abstract override RollProbabilityDistribution CreateProbabilityDistribution();
    }
}
