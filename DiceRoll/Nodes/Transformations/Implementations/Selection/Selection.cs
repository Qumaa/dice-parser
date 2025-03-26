using System;
using DiceRoll.Exceptions;

namespace DiceRoll.Nodes
{
    public sealed class Selection : MergeTransformation
    {
        private readonly SelectionType _selectionType;

        public Selection(RollProbabilityDistribution source, RollProbabilityDistribution other,
            SelectionType selectionType) : base(source, other)
        {
            EnumNotDefinedException.ThrowIfNotDefined(selectionType);
            
            _selectionType = selectionType;
        }

        protected override Probability[] AllocateProbabilitiesArray(out int outcomeToIndexOffset) =>
            new Probability[GetMax() - (outcomeToIndexOffset = GetMin()) + 1];

        protected override Probability[] GenerateProbabilities(Probability[] probabilities, int outcomeToIndexOffset)
        {
            CDFTable source = new(_source);
            CDFTable other = new(_other);

            for (int i = 0; i < probabilities.Length; i++)
            {
                Outcome outcome = new(i + outcomeToIndexOffset);

                CDF sourceCdf = CDFForOutcome(source, outcome);
                CDF otherCdf = CDFForOutcome(other, outcome);

                probabilities[i] = CDFToProbability(sourceCdf, otherCdf);
            }

            return probabilities;        }
        
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
            source.Equal * other.EqualOr +
            other.Equal * source.EqualOr -
            source.Equal * other.Equal;
    }
}
