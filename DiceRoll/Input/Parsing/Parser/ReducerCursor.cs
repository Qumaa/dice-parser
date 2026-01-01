using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    public sealed class ReducerCursor
    {
        private readonly Stack<Range> _pointers = new();

        public Range Current => _pointers.TryPeek(out Range range) ? range : Range.All;

        public void MoveTo(in Range processedSubstring) =>
            _pointers.Push(processedSubstring);

        public void MoveToPrevious() =>
            _pointers.TryPop(out _);
    }
}
