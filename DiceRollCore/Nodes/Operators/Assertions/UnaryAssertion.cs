using System;

namespace DiceRoll
{
    public abstract class UnaryAssertion : Assertion
    {
        protected readonly IAssertion _source;

        protected UnaryAssertion(IAssertion source)
        {
            ArgumentNullException.ThrowIfNull(source);
            
            _source = source;
        }

        public override void NextEvaluation() =>
            _source.NextEvaluation();
    }
}
