using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Mapped<T>
    {
        public readonly Range Range;
        public readonly T Value;

        internal Mapped(in T value, int substringStart, int substringLength) : this(
            in value,
            new Range(new Index(substringStart), new Index(substringStart + substringLength))
            ) { }

        internal Mapped(in T value, in Range range)
        {
            Value = value;
            Range = range;
        }
    }
}
