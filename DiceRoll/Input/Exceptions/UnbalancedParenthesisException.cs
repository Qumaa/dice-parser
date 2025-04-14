using System;

namespace DiceRoll.Input
{
    public sealed class UnbalancedParenthesisException : Exception
    {
        public UnbalancedParenthesisException() : base(ParsingErrorMessages.UNBALANCED_PARENTHESIS) { }
    }
}
