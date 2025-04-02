using System.Runtime.InteropServices;

namespace DiceRoll.Input
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct TokenizedOperator
    {
        public readonly IToken Token;
        public readonly int Precedence;
            
        public TokenizedOperator(IToken token, int precedence)
        {
            Token = token;
            Precedence = precedence;
        }
    }
}
