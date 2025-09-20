using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Operator
    {
        internal static readonly Operator OpenParenthesis = new();
        
        public readonly int Precedence;
        public readonly OperatorInvocationBehaviour InvocationBehaviour;

        internal bool IsOpenParenthesis => InvocationBehaviour is null;

        public Operator(int precedence, OperatorInvocationBehaviour invocationBehaviour)
        {
            Precedence = precedence;
            InvocationBehaviour = invocationBehaviour;
        }

        public Operator(OperatorDefinition definition) : this(definition.Precedence, definition.InvocationBehaviour) { }
    }
}
