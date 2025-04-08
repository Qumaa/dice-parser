using System;

namespace DiceRoll
{
    public abstract class UnaryAssertion : Assertion
    {
        protected readonly IAssertion _assertion;

        protected UnaryAssertion(IAssertion assertion)
        {
            ArgumentNullException.ThrowIfNull(assertion);
            
            _assertion = assertion;
        }
    }
}
