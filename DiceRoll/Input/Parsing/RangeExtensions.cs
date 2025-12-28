using System;

namespace DiceRoll.Input.Parsing
{
    internal static class RangeExtensions
    {
        public static (int start, int end) GetStartAndEnd(this Range range, int length)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(length);

            return range.GetStartAndEndInternal(length);
        }

        private static (int start, int end) GetStartAndEndInternal(this Range range, int length = -1)
        {
            int start = range.Start.GetOffset(length);
            int end = range.End.GetOffset(length);

            return (start, end);
        }
        
        public static Range And(this Range range, in Range other, int length = -1)
        {
            (int start1, int end1) = range.GetStartAndEndInternal(length);
            (int start2, int end2) = other.GetStartAndEndInternal(length);
            
            int start = int.Min(start1, start2);
            int end = int.Max(end1, end2);

            return start..end;
        }
    }
}
