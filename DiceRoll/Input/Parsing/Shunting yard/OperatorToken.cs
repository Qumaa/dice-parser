using System.Runtime.InteropServices;

namespace DiceRoll.Input
{
    [StructLayout(LayoutKind.Auto)]
    internal readonly struct OperatorToken
    {
        public readonly int Precedence;
        public readonly OperatorInvoker Invoker;

        public bool IsOpenParenthesis => Invoker is null;
        
        public static OperatorToken OpenParenthesis => new();

        public OperatorToken(int precedence, OperatorInvoker invoker)
        {
            Precedence = precedence;
            Invoker = invoker;
        }
    }
}
