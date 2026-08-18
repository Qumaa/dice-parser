using System;

namespace DiceRoll.Input.Parsing
{
    public class GrammarProbe
    {
        public static readonly GrammarProbe Failed = new(-1, -1);

        private readonly int _recognizedStart;
        private readonly int _recognizedLength;

        public bool IsSuccessful => _recognizedStart >= 0;
        public Range Recognized => _recognizedStart..(_recognizedStart + _recognizedLength);
        public int RecognizedStart => _recognizedStart;
        public int RecognizedLength => _recognizedLength;

        public GrammarProbe(in Substring recognized) : this(recognized.Start, recognized.Length) { }

        public GrammarProbe(int start, int length)
        {
            _recognizedStart = start;
            _recognizedLength = length;
        }
    }

    public static class GrammarProbeExtensions
    {
        public static GrammarProbe Merge(this GrammarProbe probe, GrammarProbe other)
        {
            if (!(probe.IsSuccessful && other.IsSuccessful))
                return GrammarProbe.Failed;

            int newStart = Math.Min(probe.RecognizedStart, other.RecognizedStart);
            int thisEnd = probe.RecognizedStart + probe.RecognizedLength;
            int otherEnd = other.RecognizedStart + other.RecognizedLength;
            int newEnd = Math.Max(thisEnd, otherEnd);
            int newLength = newEnd - newStart;

            return new GrammarProbe(newStart, newLength);
        }
    }
}
