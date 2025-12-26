using System;

namespace DiceRoll.Input.Parsing
{
    internal static class RangeExtensions
    {
        public static (int start, int end) GetStartAndEnd(this Range range, int length = -1)
        {
            int start = range.Start.GetOffset(length);
            int end = range.End.GetOffset(length);

            return (start, end);
        }
        
        public static Range And(this Range range, in Range other, int length = -1)
        {
            (int start1, int end1) = range.GetStartAndEnd(length);
            (int start2, int end2) = other.GetStartAndEnd(length);
            
            int start = int.Min(start1, start2);
            int end = int.Max(end1, end2);

            return start..end;
        }

        public static bool OverlapsWith(this Range range, in Range other, int length = -1)
        {
            (int start1, int end1) = range.GetStartAndEnd(length);
            (int start2, int end2) = other.GetStartAndEnd(length);
            
            return start1 <= end2 && start2 <= end1;
        }

        public static bool FitsIn(this Range range, in Range other, int length = -1)
        {
            (int start1, int end1) = range.GetStartAndEnd(length);
            (int start2, int end2) = other.GetStartAndEnd(length);

            return start1 >= start2 && end1 <= end2;
        }
    }
}
