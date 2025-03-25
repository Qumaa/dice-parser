using System;
using System.Linq;

namespace DiceRoll.Expressions
{
    public sealed class Selection : ProbabilityDistributionTransformation
    {
        private readonly RollProbabilityDistribution _other;
        private readonly SelectionType _selectionType;

        public Selection(RollProbabilityDistribution source, RollProbabilityDistribution other,
            SelectionType selectionType) : base(source)
        {
            _other = other;
            _selectionType = selectionType;
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

        private int GetMax() =>
        _selectionType is SelectionType.Highest ?
            Math.Max(_source.Max.Value, _other.Max.Value) :
            Math.Min(_source.Max.Value, _other.Max.Value);

        private Probability GetSecondCDFValue(CDFTable cdfTable, Outcome outcome) =>
            _selectionType is SelectionType.Highest ?
                cdfTable.LessThanOrEqualTo(outcome) :
                cdfTable.GreaterThanOrEqualTo(outcome);

        private static Probability CDFToProbability(CDF source, CDF other) =>
            new(source.Equal.Value * other.EqualOr.Value +
                other.Equal.Value * source.EqualOr.Value -
                source.Equal.Value * other.Equal.Value);
    }
}
