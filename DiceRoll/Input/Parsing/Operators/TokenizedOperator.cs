using System.Runtime.InteropServices;

namespace DiceRoll.Input
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct TokenizedOperator
    {
        public readonly IToken Token;
        public readonly int Precedence;
        public readonly OperatorParser Parser;
            
        public TokenizedOperator(IToken token, int precedence, OperatorParser parser)
        {
            Token = token;
            Precedence = precedence;
            Parser = parser;
        }
    }
}
