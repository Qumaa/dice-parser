using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct CompositionToken
    {
        public readonly IToken Token;
        public readonly CompositionHandler CompositionHandler;

        public CompositionToken(IToken token, CompositionHandler compositionHandler)
        {
            Token = token;
            CompositionHandler = compositionHandler;
        }
    }
}
