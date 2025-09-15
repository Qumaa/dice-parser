using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    internal readonly struct DelayedOperator
    {
        public readonly OperatorInvoker Invoker;
        public readonly int CapturedParenthesisLevel;
        public readonly int CapturedOperands;
            
        public DelayedOperator(OperatorInvoker invoker, int capturedParenthesisLevel, int capturedOperands)
        {
            Invoker = invoker;
            CapturedParenthesisLevel = capturedParenthesisLevel;
            CapturedOperands = capturedOperands;
        }
    }
}
