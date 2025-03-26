using System;
using System.Linq;

namespace DiceRoll.Nodes
{
    public abstract class MergeTransformation : Transformation
    {
        protected readonly RollProbabilityDistribution _other;

        protected MergeTransformation(RollProbabilityDistribution source,
            RollProbabilityDistribution other) : base(source)
        {
            ArgumentNullException.ThrowIfNull(other);
            
            _other = other;
        }
        
        public sealed override RollProbabilityDistribution Evaluate() =>
            Distribute(
                GenerateProbabilities(
                    AllocateProbabilitiesArray(out int valueToIndexOffset), 
                    valueToIndexOffset
                ),
                valueToIndexOffset
            );

        protected abstract Probability[] GenerateProbabilities(Probability[] probabilities, int outcomeToIndexOffset);
        
        protected abstract Probability[] AllocateProbabilitiesArray(out int outcomeToIndexOffset);
        
        private static RollProbabilityDistribution Distribute(Probability[] probabilities, int outcomeToIndexOffset) =>
            new(probabilities.Select((d, i) => new Roll(i + outcomeToIndexOffset, d)));
    }
}
