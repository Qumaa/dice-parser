using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    internal readonly struct OperatorToken
    {
        public static readonly OperatorToken OpenParenthesis = new();
        
        public readonly int Precedence;
        public readonly OperatorInvoker Invoker;

        public bool IsOpenParenthesis => Invoker is null;

        public OperatorToken(int precedence, OperatorInvoker invoker)
        {
            Precedence = precedence;
            Invoker = invoker;
        }
    }
}
