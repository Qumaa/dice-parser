using System.Linq;

namespace DiceRoll
{
    public sealed class Selection : BinaryTransformation
    {
        private readonly SelectionType _selectionType;

        public Selection(INumeric source, INumeric other, SelectionType selectionType) : base(source, other)
        {
            EnumValueNotDefinedException.ThrowIfValueNotDefined(selectionType);
            
            _selectionType = selectionType;
        }

        public override Outcome Evaluate() =>
            _selectionType is SelectionType.Highest ?
                Outcome.Max(_source.Evaluate(), _other.Evaluate()) : 
                Outcome.Min(_source.Evaluate(), _other.Evaluate());

        protected override RollProbabilityDistribution CreateProbabilityDistribution()
        {
            RollProbabilityDistribution source = _source.GetProbabilityDistribution();
            RollProbabilityDistribution other = _other.GetProbabilityDistribution();
            
            CDFTable sourceTable = new(source);
            CDFTable otherTable = new(other);

            return source
                .Union(other)
                .Select(
                    outcome =>
                        new Roll(
                            outcome,
                            CDFToProbability(
                                CDFForOutcome(sourceTable, outcome),
                                CDFForOutcome(otherTable, outcome)
                                )
                            )
                    )
                .ToRollProbabilityDistribution();
        }

        private CDF CDFForOutcome(CDFTable cdfTable, Outcome outcome) =>
            new(cdfTable.EqualTo(outcome), GetSecondCDFValue(cdfTable, outcome));

        private Probability GetSecondCDFValue(CDFTable cdfTable, Outcome outcome) =>
            _selectionType is SelectionType.Highest ?
                cdfTable.LessThanOrEqual(outcome) :
                cdfTable.GreaterThanOrEqual(outcome);

        private static Probability CDFToProbability(CDF source, CDF other) =>
            source.Equal * other.EqualOr +
            other.Equal * source.EqualOr -
            source.Equal * other.Equal;
    }
}
