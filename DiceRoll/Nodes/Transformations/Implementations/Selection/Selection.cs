using System.Linq;
using DiceRoll.Exceptions;

namespace DiceRoll.Nodes
{
    public sealed class Selection : MergeTransformation
    {
        private readonly SelectionType _selectionType;

        public Selection(IAnalyzable source, IAnalyzable other, SelectionType selectionType) : base(source, other)
        {
            EnumValueNotDefinedException.ThrowIfValueNotDefined(selectionType);
            
            _selectionType = selectionType;
        }

        public override Outcome Evaluate() =>
            _selectionType is SelectionType.Highest ?
                Outcome.Max(_source.Evaluate(), _other.Evaluate()) : 
                Outcome.Min(_source.Evaluate(), _other.Evaluate());

        public override RollProbabilityDistribution GetProbabilityDistribution()
        {
            RollProbabilityDistribution source = _source.GetProbabilityDistribution();
            RollProbabilityDistribution other = _other.GetProbabilityDistribution();
            
            CDFTable sourceTable = new(source);
            CDFTable otherTable = new(other);

            return new RollProbabilityDistribution(source
                .Union(other)
                .Select(outcome =>
                    new Roll(
                        outcome,
                        CDFToProbability(
                            CDFForOutcome(sourceTable, outcome), 
                            CDFForOutcome(otherTable, outcome)
                            )
                        )
                )
            );
        }

        private CDF CDFForOutcome(CDFTable cdfTable, Outcome outcome) =>
            new(cdfTable.EqualTo(outcome), GetSecondCDFValue(cdfTable, outcome));

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
