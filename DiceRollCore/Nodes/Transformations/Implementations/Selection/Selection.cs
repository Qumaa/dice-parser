using System.Linq;

namespace DiceRoll
{
    /// <summary>
    /// Merges two arbitrary <see cref="INumeric">numeric nodes</see> by selecting one of their
    /// <see cref="Outcome"/> and provides an updated
    /// <see cref="RollProbabilityDistribution">probability distribution</see> of the results.
    /// </summary>
    /// <seealso cref="SelectionType"/>
    public sealed class Selection : BinaryTransformation
    {
        private readonly SelectionType _selectionType;

        /// <inheritdoc cref="BinaryTransformation(DiceRoll.INumeric,DiceRoll.INumeric)"/>
        /// <param name="selectionType">The type of selection.</param>
        /// <exception cref="EnumValueNotDefinedException">
        /// When <paramref name="selectionType"/> holds a not defined value.
        /// </exception>
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
