using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    public sealed class OperatorDefinition
    {
        public readonly IToken Token;
        public readonly OperatorInvocationBehaviour InvocationBehaviour;

        public OperatorDefinition(IToken token, OperatorInvocationBehaviour invocationBehaviour)
        {
            Token = token;
            InvocationBehaviour = invocationBehaviour;
        }
    }
}
