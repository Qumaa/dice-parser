using System;

namespace DiceRoll.Input
{
    public sealed class UnbalancedParenthesisException : Exception
    {
        private const string _MESSAGE = "Couldn't close parenthesis: unbalanced parenthesis within the expression.";

        public UnbalancedParenthesisException() : base(_MESSAGE) { }
    }
}
