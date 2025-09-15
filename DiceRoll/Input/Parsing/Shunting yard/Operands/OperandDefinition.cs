using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct OperandDefinition
    {
        public readonly IToken Token;
        public readonly OperandHandler Handler;
            
        public OperandDefinition(IToken token, OperandHandler handler)
        {
            Token = token;
            Handler = handler;
        }

        public INode Parse(Substring match) =>
            Handler(match);

        public bool TryParse(Substring match, out INode node)
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
