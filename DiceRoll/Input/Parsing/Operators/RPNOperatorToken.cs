using System.Runtime.InteropServices;

namespace DiceRoll.Input
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct RPNOperatorToken
    {
        public readonly int Precedence;
        public readonly RPNOperatorInvoker Invoker;

        public bool IsOpenParenthesis => Invoker is null;
        
        public static RPNOperatorToken OpenParenthesis => new();

        public RPNOperatorToken(int precedence, RPNOperatorInvoker invoker)
        {
            Precedence = precedence;
            Invoker = invoker;
        }
    }
}
