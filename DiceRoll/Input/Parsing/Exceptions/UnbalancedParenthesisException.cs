using System;

namespace DiceRoll.Input.Parsing
{
    internal sealed class UnbalancedParenthesisException : Exception
    {
        public UnbalancedParenthesisException() : base("This parenthesis has no matching opening pair.") { }
    }
}
