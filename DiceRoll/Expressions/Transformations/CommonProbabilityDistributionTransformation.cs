using System.Linq;

namespace DiceRoll.Expressions
{
    public abstract class CommonProbabilityDistributionTransformation : ProbabilityDistributionTransformation
    {
        protected readonly RollProbabilityDistribution _other;

        protected CommonProbabilityDistributionTransformation(RollProbabilityDistribution source,
            RollProbabilityDistribution other) : base(source)
        {
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

        protected abstract Probability[] GenerateProbabilities(Probability[] probabilities, int valueToIndexOffset);
        
        protected abstract Probability[] AllocateProbabilitiesArray(out int valueToIndexOffset);
        
        private static RollProbabilityDistribution Distribute(Probability[] probabilities, int valueToIndexOffset) =>
            new(probabilities.Select((d, i) => new Roll(i + valueToIndexOffset, d)));
    }
}
