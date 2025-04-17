using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class UnbalancedParenthesisException : Exception
    {
        public UnbalancedParenthesisException() : base(ParsingErrorMessages.UNBALANCED_PARENTHESIS) { }
    }
}
