using System;

namespace DiceRoll
{
    public abstract class UnaryAssertion : Assertion
    {
        public readonly IAssertion Source;

        protected UnaryAssertion(IAssertion source)
        {
            ArgumentNullException.ThrowIfNull(source);
            
            Source = source;
        }

        public override void NextEvaluation() =>
            Source.NextEvaluation();
    }
}
