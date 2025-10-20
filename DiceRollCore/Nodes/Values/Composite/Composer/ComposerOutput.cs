using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DiceRoll
{
    [StructLayout(LayoutKind.Auto)]
    public readonly ref struct ComposerOutput
    {
        public readonly INumeric CompositeNumeric;
        public readonly IEnumerable<INumeric> SourceNodes;
            
        public ComposerOutput(INumeric compositeNumeric, IEnumerable<INumeric> sourceNodes)
        {
            CompositeNumeric = compositeNumeric;
            SourceNodes = sourceNodes;
        }
    }
}
