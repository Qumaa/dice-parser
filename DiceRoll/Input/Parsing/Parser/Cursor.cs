using System;
using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public sealed class Cursor
    {
        private readonly Stack<Range> _pointers = new();

        public Range Current => _pointers.TryPeek(out Range range) ? range : Range.All;

        public void MoveTo(in Range processedSubstring) =>
            _pointers.Push(processedSubstring);

        public void MoveToPrevious() =>
            _pointers.TryPop(out _);

        public void Clear() =>
            _pointers.Clear();
    }

    public static class CursorExtensions
    {
        public static Substring GetSubstringOfCurrent(this Cursor cursor, SubstringMapper mapper) =>
            mapper.GetSubstringOf(cursor.Current);
        
        public static Substring GetSubstringOfCurrent(this Cursor cursor, InputMapper mapper)
        {
            int length = mapper.InputLength;
            (int _, int currentLength) = cursor.Current.GetOffsetAndLength(length);

            Range currentRange = (length - currentLength)..length;
            
            return mapper.GetSubstringOf(currentRange);
        }
    }
}
