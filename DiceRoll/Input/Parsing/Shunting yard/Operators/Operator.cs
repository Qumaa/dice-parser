using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing.Deprecated
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Operator
    {
        public readonly OperatorInvocationBehaviour InvocationBehaviour;
        public readonly int Precedence;
        public readonly int Position;
        
        internal bool IsOpenParenthesis => InvocationBehaviour is null;
            
        public Operator(OperatorInvocationBehaviour behaviour, int precedence, int position)
        {
            InvocationBehaviour = behaviour;
            Position = position;
            Precedence = precedence;
        }
    }
}
