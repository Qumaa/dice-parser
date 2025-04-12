using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct FormulaSubstring<T>
    {
        public readonly Range Range;
        public readonly T Value;
            
        public FormulaSubstring(in T value, int contextStart, int contextLength)
        {
            Value = value;
            Range = new Range(new Index(contextStart), new Index(contextStart + contextLength));
        }
    }
}
