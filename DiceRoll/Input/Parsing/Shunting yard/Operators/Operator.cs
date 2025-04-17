using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Operator
    {
        public readonly IToken Token;
        public readonly int Precedence;
        public readonly OperatorInvoker Invoker;
            
        public Operator(IToken token, int precedence, OperatorInvoker invoker)
        {
            Token = token;
            Precedence = precedence;
            Invoker = invoker;
        }
    }
}
