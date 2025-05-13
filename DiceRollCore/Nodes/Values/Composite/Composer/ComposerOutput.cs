using System.Runtime.InteropServices;

namespace DiceRoll
{
    [StructLayout(LayoutKind.Auto)]
    public readonly ref struct ComposerOutput
    {
        public readonly INumeric CompositeNode;
        public readonly ComposerContext Context;
        
        public ComposerOutput(INumeric compositeNode, ComposerContext context)
        {
            CompositeNode = compositeNode;
            Context = context;
        }
    }
}
