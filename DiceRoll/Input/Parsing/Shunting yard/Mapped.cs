using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Mapped<T>
    {
        public readonly Range Range;
        public readonly T Value;
            
        public Mapped(in T value, int contextStart, int contextLength)
        {
            Value = value;
            Range = new Range(new Index(contextStart), new Index(contextStart + contextLength));
        }
    }
}
