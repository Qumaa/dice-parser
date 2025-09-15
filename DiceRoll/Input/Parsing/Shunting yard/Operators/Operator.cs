using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    internal readonly struct Operator
    {
        public static readonly Operator OpenParenthesis = new();
        
        public readonly int Precedence;
        public readonly OperatorInvoker Invoker;

        public bool IsOpenParenthesis => Invoker is null;

        public Operator(int precedence, OperatorInvoker invoker)
        {
            Precedence = precedence;
            Invoker = invoker;
        }
    }
}
