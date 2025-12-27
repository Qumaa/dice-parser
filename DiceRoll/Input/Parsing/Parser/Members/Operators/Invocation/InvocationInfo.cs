using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    internal readonly struct InvocationInfo
    {
        public readonly OperatorInvoker Invoker;
        public readonly Mapped<Operand>[] Operands;
            
        public InvocationInfo(OperatorInvoker invoker, Mapped<Operand>[] operands)
        {
            Invoker = invoker;
            Operands = operands;
        }
    }
}
