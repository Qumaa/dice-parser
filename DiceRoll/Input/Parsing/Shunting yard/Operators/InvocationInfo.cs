using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    internal readonly struct InvocationInfo
    {
        public readonly OperatorInvoker Invoker;
        public readonly Mapped<LinkedNode>[] CastedOperands;
            
        public InvocationInfo(OperatorInvoker invoker, Mapped<LinkedNode>[] castedOperands)
        {
            Invoker = invoker;
            CastedOperands = castedOperands;
        }
    }
}
