using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    internal readonly struct Operator
    {
        public static readonly Operator OpenParenthesis = new();
        
        public readonly int Precedence;
        public readonly OperatorInvocationBehaviour InvocationBehaviour;

        public bool IsOpenParenthesis => InvocationBehaviour is null;

        public Operator(int precedence, OperatorInvocationBehaviour invocationBehaviour)
        {
            Precedence = precedence;
            InvocationBehaviour = invocationBehaviour;
        }
    }
}
