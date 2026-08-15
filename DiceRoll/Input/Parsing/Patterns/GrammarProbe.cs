using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct GrammarProbe
    {
        public static readonly GrammarProbe Failed = new(-1, -1);

        private readonly int _recognizedStart;
        private readonly int _recognizedLength;

        public bool IsSuccessful => _recognizedStart >= 0;
        public Range Recognized => _recognizedStart..(_recognizedStart + _recognizedLength);

        public GrammarProbe(in Substring recognized) : this(recognized.Start, recognized.Length) { }

        private GrammarProbe(int start, int length)
        {
            _recognizedStart = start;
            _recognizedLength = length;
        }

        public GrammarProbe Merge(in GrammarProbe other)
        {
            if (!other.IsSuccessful)
                return Failed;

            int newStart = Math.Min(_recognizedStart, other._recognizedStart);
            int thisEnd = _recognizedStart + _recognizedLength;
            int otherEnd = other._recognizedStart + other._recognizedLength;
            int newEnd = Math.Max(thisEnd, otherEnd);
            int newLength = newEnd - newStart;

            return new GrammarProbe(newStart, newLength);
        }
    }
}
