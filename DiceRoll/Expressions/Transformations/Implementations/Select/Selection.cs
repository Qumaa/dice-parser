using System;
using System.Linq;

namespace DiceRoll.Expressions
{
    public sealed class Selection : ProbabilityDistributionTransformation
    {
        private readonly ProbabilityDistribution _other;
        private readonly SelectMode _selectMode;

        public Selection(ProbabilityDistribution source, ProbabilityDistribution other, SelectMode selectMode) : base(source)
        {
            _other = other;
            _selectMode = selectMode;
        }

        public override ProbabilityDistribution Evaluate()
        {
            Probability[] probabilities = AllocateProbabilitiesArray(out int indexToValueOffset);
            FillProbabilities(probabilities, indexToValueOffset);

            return new ProbabilityDistribution(probabilities.Select((x, i) => new Roll(i + indexToValueOffset, x)));
        }

        private Probability[] AllocateProbabilitiesArray(out int offset)
        {
            int min = Math.Min(_source.Min.Value, _other.Min.Value);
            int max = _selectMode is SelectMode.Lowest ? 
                Math.Min(_source.Max.Value, _other.Max.Value) : 
                Math.Max(_source.Max.Value, _other.Max.Value);

            offset = min;
            return new Probability[max - min + 1];
        }

        private void FillProbabilities(Probability[] probabilities, int offset)
        {
            CDFProvider source = GetCDFProvider(_source);
            CDFProvider other = GetCDFProvider(_other);

            for (int i = 0; i < probabilities.Length; i++)
            {
                Outcome outcome = new(i + offset);

                CDF sourceCdf = source.ForOutcome(outcome);
                CDF otherCdf = other.ForOutcome(outcome);

                probabilities[i] = CDFToProbability(sourceCdf, otherCdf);
            }
        }

        private CDFProvider GetCDFProvider(ProbabilityDistribution distribution) =>
            _selectMode is SelectMode.Highest ? new CDFTable(distribution) : new CDFTableReversed(distribution);

        private static Probability CDFToProbability(CDF source, CDF other) =>
            new(source.Equal.Value * other.EqualOr.Value +
                other.Equal.Value * source.EqualOr.Value -
                source.Equal.Value * other.Equal.Value);
    }
}
