using System;

namespace DiceRoll.Input
{
    internal sealed class UnbalancedParenthesisException : Exception
    {
        private const string _MESSAGE = "Couldn't close parenthesis: unbalanced parenthesis within the expression.";
        
        public UnbalancedParenthesisException() : base(_MESSAGE) { }
        public UnbalancedParenthesisException(Exception innerException) : base(_MESSAGE, innerException) { }
    }
}
