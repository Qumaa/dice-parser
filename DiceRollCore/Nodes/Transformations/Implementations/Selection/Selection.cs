using System.Linq;

namespace DiceRoll
{
    public sealed class Selection : BinaryTransformation
    {
        private readonly SelectionType _selectionType;

        public Selection(INumeric left, INumeric right, SelectionType selectionType) : base(left, right)
        {
            EnumValueNotDefinedException.ThrowIfValueNotDefined(selectionType);
            
            _selectionType = selectionType;
        }

        public override void NextEvaluation()
        {
            Outcome left = _left.Evaluate();
            Outcome right = _right.Evaluate();
            
            Outcome evaluation = _selectionType is SelectionType.Highest ?
                Outcome.Max(left, right) : 
                Outcome.Min(left, right);
            
            CacheEvaluation(in evaluation);
        }

        protected override RollProbabilityDistribution CreateProbabilityDistribution()
        {
            RollProbabilityDistribution source = _left.GetProbabilityDistribution();
            RollProbabilityDistribution other = _right.GetProbabilityDistribution();
            
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
