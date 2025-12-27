using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    public sealed class OperatorDefinition
    {
        public readonly IToken Token;
        public readonly int Precedence;
        public readonly OperatorInvocationBehaviour InvocationBehaviour;
        public readonly Associativity Associativity;

        public OperatorDefinition(IToken token, int precedence, OperatorInvocationBehaviour invocationBehaviour,
            Associativity associativity)
        {
            Token = token;
            Precedence = precedence;
            InvocationBehaviour = invocationBehaviour;
            Associativity = associativity;
        }
    }
}
