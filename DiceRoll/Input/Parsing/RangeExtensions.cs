using System;

namespace DiceRoll.Input.Parsing
{
    internal static class RangeExtensions
    {
        public static Range And(this Range range, in Range other)
        {
            int start = int.Min(range.Start.Value, other.Start.Value);
            int end = int.Max(range.End.Value, other.End.Value);

            return new Range(new Index(start), new Index(end));
        }

        public static bool OverlapsWith(this Range range, in Range other, int length = 0)
        {
            bool hasLength = length > 0;

            int start1 = hasLength ? range.Start.GetOffset(length) : range.Start.Value;
            int end1 = hasLength ? range.End.GetOffset(length) : range.End.Value;
            
            int start2 = hasLength ? other.Start.GetOffset(length) : other.Start.Value;
            int end2 = hasLength ? other.End.GetOffset(length) : other.End.Value;
            
            return start1 <= end2 && start2 <= end1;
        }
    }
}
