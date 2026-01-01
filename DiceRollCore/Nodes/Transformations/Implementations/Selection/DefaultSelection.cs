using System.Linq;
using DiceRoll.Exceptions;

namespace DiceRoll
{
    public sealed class DefaultSelection : BinaryTransformation
    {
        public readonly SelectionType SelectionType;

        public DefaultSelection(INumeric left, INumeric right, SelectionType selectionType) : base(left, right)
        {
            EnumValueNotDefinedException.ThrowIfValueNotDefined(selectionType);
            
            SelectionType = selectionType;
        }

        public override void NextEvaluation()
        {
            Outcome left = Left.Evaluate();
            Outcome right = Right.Evaluate();
            
            Outcome evaluation = SelectionType is SelectionType.Highest ?
                Outcome.Max(left, right) : 
                Outcome.Min(left, right);
            
            CacheEvaluation(in evaluation);
        }

        protected override RollProbabilityDistribution CreateProbabilityDistribution()
        {
            RollProbabilityDistribution source = Left.GetProbabilityDistribution();
            RollProbabilityDistribution other = Right.GetProbabilityDistribution();
            
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
            SelectionType is SelectionType.Highest ?
                cdfTable.LessThanOrEqual(outcome) :
                cdfTable.GreaterThanOrEqual(outcome);

        private static Probability CDFToProbability(CDF source, CDF other) =>
            source.Equal * other.EqualOr +
            other.Equal * source.EqualOr -
            source.Equal * other.Equal;
    }
}
