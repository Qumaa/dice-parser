using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Operator
    {
        internal static readonly Operator OpenParenthesis = new();
        
        public readonly OperatorInvocationBehaviour InvocationBehaviour;
        public readonly int Precedence;
        public readonly int OperandsPosition;
        
        internal bool IsOpenParenthesis => InvocationBehaviour is null;
            
        public Operator(OperatorInvocationBehaviour invocationBehaviour, int precedence, int operandsPosition)
        {
            InvocationBehaviour = invocationBehaviour;
            OperandsPosition = operandsPosition;
            Precedence = precedence;
        }
    }
}
