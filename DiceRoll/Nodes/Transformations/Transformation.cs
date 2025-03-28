using System;

namespace DiceRoll.Nodes
{
    public abstract class Transformation : IAnalyzable
    {
        protected readonly IAnalyzable _source;
        
        protected Transformation(IAnalyzable source)
        {
            ArgumentNullException.ThrowIfNull(source);
            
            _source = source;
        }

        public abstract Outcome Evaluate();
        
        public abstract RollProbabilityDistribution GetProbabilityDistribution();
    }
}
