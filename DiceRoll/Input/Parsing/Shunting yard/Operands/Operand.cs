using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Operand
    {
        public readonly IToken Token;
        public readonly OperandHandler Handler;
            
        public Operand(IToken token, OperandHandler handler)
        {
            Token = token;
            Handler = handler;
        }

        public INumeric Parse(Substring match) =>
            Handler(match);

        public bool TryParse(Substring match, out INumeric node)
        {
            if (!Token.Matches(match))
            {
                node = null;
                return false;
            }

            node = Parse(match);
            return true;
        }
    }
}
