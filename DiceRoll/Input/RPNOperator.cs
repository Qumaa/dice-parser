using System.Runtime.InteropServices;

namespace DiceRoll.Input
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct RPNOperator
    {
        public readonly string Token;
        public readonly int Precedence;
        
        public RPNOperator(string token, int precedence = 0)
        {
            Token = token;
            Precedence = precedence;
        }
    }
}
