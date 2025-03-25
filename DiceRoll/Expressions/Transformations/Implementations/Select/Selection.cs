using System;
using System.Linq;

namespace DiceRoll.Expressions
{
    public abstract class Selection : ProbabilityDistributionTransformation
    {
        protected readonly RollProbabilityDistribution _other;

        protected Selection(RollProbabilityDistribution source, RollProbabilityDistribution other) : base(source)
        {
            _other = other;
        }

        public override RollProbabilityDistribution Evaluate()
        {
            Probability[] probabilities = AllocateProbabilitiesArray(out int indexToValueOffset);
            FillProbabilities(probabilities, indexToValueOffset);

            return new RollProbabilityDistribution(probabilities.Select((x, i) => new Roll(i + indexToValueOffset, x)));
        }

        private Probability[] AllocateProbabilitiesArray(out int offset)
        {
            int min = GetMin();
            int max = GetMax();

            offset = min;
            return new Probability[max - min + 1];
        }

        private void FillProbabilities(Probability[] probabilities, int offset)
        {
            CDFTable source = new(_source);
            CDFTable other = new(_other);

            for (int i = 0; i < probabilities.Length; i++)
            {
                Outcome outcome = new(i + offset);

                CDF sourceCdf = CDFForOutcome(source, outcome);
                CDF otherCdf = CDFForOutcome(other, outcome);

                probabilities[i] = CDFToProbability(sourceCdf, otherCdf);
            }
        }

        private CDF CDFForOutcome(CDFTable cdfTable, Outcome outcome) =>
            new(cdfTable.EqualTo(outcome), GetSecondCDFValue(cdfTable, outcome));

        private int GetMin() =>
            Math.Min(_source.Min.Value, _other.Min.Value);

        protected abstract int GetMax();

        protected abstract Probability GetSecondCDFValue(CDFTable cdfTable, Outcome outcome);

        private static Probability CDFToProbability(CDF source, CDF other) =>
            new(source.Equal.Value * other.EqualOr.Value +
                other.Equal.Value * source.EqualOr.Value -
                source.Equal.Value * other.Equal.Value);
    }

    public sealed class SelectHighest : Selection
    {
        public SelectHighest(RollProbabilityDistribution source, RollProbabilityDistribution other) : 
            base(source, other) { }

        protected override int GetMax() =>
            Math.Max(_source.Max.Value, _other.Max.Value);

        protected override Probability GetSecondCDFValue(CDFTable cdfTable, Outcome outcome) =>
            cdfTable.LessOrEqualThan(outcome);
    }

    public sealed class SelectLowest : Selection
    {
        public SelectLowest(RollProbabilityDistribution source, RollProbabilityDistribution other) : 
            base(source, other) { }

        protected override int GetMax() =>
            Math.Min(_source.Max.Value, _other.Max.Value);

        protected override Probability GetSecondCDFValue(CDFTable cdfTable, Outcome outcome) =>
            cdfTable.GreaterOrEqualThan(outcome);
    }
}
