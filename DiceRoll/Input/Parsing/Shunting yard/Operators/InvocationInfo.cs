using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    internal readonly struct InvocationInfo
    {
        public readonly OperatorInvoker Invoker;
        public readonly OperandCaster[] Casters;
            
        public InvocationInfo(OperatorInvoker invoker, OperandCaster[] casters)
        {
            Invoker = invoker;
            Casters = casters;
        }
    }
}
