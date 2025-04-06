using System.Linq;

namespace DiceRoll
{
    public sealed class Conditional : IConditional
    {
        private readonly IOperation _condition;
        private readonly INumeric _value;
        
        public Conditional(IOperation condition, INumeric value)
        {
            _condition = condition;
            _value = value;
        }

        public Optional<Outcome> Evaluate() =>
            _condition.Evaluate() ? new Optional<Outcome>(_value.Evaluate()) : Optional<Outcome>.Empty;

        public OptionalRollProbabilityDistribution GetProbabilityDistribution()
        {
            Probability ofTrue = _condition.GetProbabilityDistribution().True;

            return new OptionalRollProbabilityDistribution(_value.GetProbabilityDistribution()
                .Select(x => new OptionalRoll(x.Outcome, x.Probability * ofTrue))
                .Prepend(new OptionalRoll(ofTrue.Inversed())
                )
            );
        }

        public void Visit(INodeVisitor visitor) =>
            visitor.ForConditional(this);
    }
}
