using System;

namespace DiceRoll.Nodes
{
    public abstract class Transformation : INode<RollProbabilityDistribution>
    {
        protected readonly RollProbabilityDistribution _source;
        
        protected Transformation(RollProbabilityDistribution source)
        {
            ArgumentNullException.ThrowIfNull(source);
            
            _source = source;
        }

        public abstract RollProbabilityDistribution Evaluate();
    }
}
