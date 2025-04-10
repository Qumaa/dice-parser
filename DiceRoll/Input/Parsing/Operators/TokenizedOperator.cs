using System.Runtime.InteropServices;

namespace DiceRoll.Input
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct TokenizedOperator
    {
        public readonly IToken Token;
        public readonly int Precedence;
        public readonly ShuntingYard.OperatorInvoker Invoker;
            
        public TokenizedOperator(IToken token, int precedence, ShuntingYard.OperatorInvoker invoker)
        {
            Token = token;
            Precedence = precedence;
            Invoker = invoker;
        }
    }
}
