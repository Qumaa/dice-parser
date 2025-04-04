using System.Runtime.InteropServices;

namespace DiceRoll.Input
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct RPNOperatorToken
    {
        public readonly int Precedence;
        public readonly RPNOperatorParser Parser;

        public bool IsOpenParenthesis => Parser is null;
        
        public static RPNOperatorToken OpenParenthesis => new();

        public RPNOperatorToken(int precedence, RPNOperatorParser parser)
        {
            Precedence = precedence;
            Parser = parser;
        }
    }
}
