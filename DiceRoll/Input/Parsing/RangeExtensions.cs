using System;

namespace DiceRoll.Input.Parsing
{
    internal static class RangeExtensions
    {
        public static Range And(this in Range range, in Range other)
        {
            int start = int.Min(range.Start.Value, other.Start.Value);
            int end = int.Max(range.End.Value, other.End.Value);

            return new Range(new Index(start), new Index(end));
        }
    }
}
