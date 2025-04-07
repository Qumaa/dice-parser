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
            return _condition.GetOptionalRollsProbabilityDistribution();
            
            Probability ofFalse = _condition.GetProbabilityDistribution().False;

            return new OptionalRollProbabilityDistribution(_condition.GetOptionalRollsProbabilityDistribution()
                .Prepend(new OptionalRoll(ofFalse)
                )
            );
        }

        public void Visit(INodeVisitor visitor) =>
            visitor.ForConditional(this);
    }
}
