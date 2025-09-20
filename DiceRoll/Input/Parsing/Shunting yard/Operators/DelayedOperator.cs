using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    internal readonly struct DelayedOperator
    {
        public readonly OperatorInvocationBehaviour InvocationBehaviour;
        public readonly int CapturedParenthesisLevel;
        public readonly int CapturedOperands;
            
        public DelayedOperator(OperatorInvocationBehaviour invocationBehaviour, int capturedParenthesisLevel, int capturedOperands)
        {
            InvocationBehaviour = invocationBehaviour;
            CapturedParenthesisLevel = capturedParenthesisLevel;
            CapturedOperands = capturedOperands;
        }
    }
}
