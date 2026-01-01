using System;
using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public sealed class Cursor
    {
        private readonly SubstringMapper _mapper;
        private readonly Stack<Range> _pointers;

        public Cursor(SubstringMapper mapper)
        {
            _mapper = mapper;
            _pointers = new Stack<Range>();
        }

        public Range Current => _pointers.TryPeek(out Range range) ? range : Range.All;

        public void MoveTo(in Range processedSubstring) =>
            _pointers.Push(processedSubstring);

        public void MoveToPrevious() =>
            _pointers.TryPop(out _);

        public Substring GetSubstringOfCurrent() =>
            _mapper.GetSubstring(Current);
    }
}
