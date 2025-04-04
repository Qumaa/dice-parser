using System.Runtime.InteropServices;

namespace DiceRoll.Input
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct DiceCompositionToken
    {
        public readonly IToken Token;
        public readonly DiceCompositionHandler CompositionHandler;

        public DiceCompositionToken(IToken token, DiceCompositionHandler compositionHandler)
        {
            Token = token;
            CompositionHandler = compositionHandler;
        }
    }
}
