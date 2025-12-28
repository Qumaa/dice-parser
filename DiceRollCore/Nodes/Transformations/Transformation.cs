using System;

namespace DiceRoll
{
    public abstract class Transformation : Numeric
    {
        public readonly INumeric Node;
        
        protected Transformation(INumeric node)
        {
            ArgumentNullException.ThrowIfNull(node);
            
            Node = node;
        }
        
        protected abstract override RollProbabilityDistribution CreateProbabilityDistribution();

        public override void NextEvaluation() =>
            Node.NextEvaluation();
    }
}
