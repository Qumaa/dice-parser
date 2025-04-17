using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Mapped<T>
    {
        public readonly Range Range;
        public readonly T Value;

        public Mapped(in T value, int contextStart, int contextLength) : this(
            in value,
            new Range(new Index(contextStart), new Index(contextStart + contextLength))
            ) { }

        public Mapped(in T value, Range range)
        {
            Value = value;
            Range = range;
        }

        public Range Merge(in Mapped<T> other) =>
            Merge(in other.Range);
        
        public Range Merge(in Range other)
        {
            int start = int.Min(Range.Start.Value, other.Start.Value);
            int end = int.Max(Range.End.Value, other.End.Value);

            return new Range(new Index(start), new Index(end));
        }
    }
}
