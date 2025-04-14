using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct FormulaToken<T>
    {
        public readonly Range Range;
        public readonly T Value;
            
        public FormulaToken(in T value, int contextStart, int contextLength)
        {
            Value = value;
            Range = new Range(new Index(contextStart), new Index(contextStart + contextLength));
        }

        public static FormulaToken<T> NotTokenized(in T value) =>
            new(value, 0, 0);
    }
}
