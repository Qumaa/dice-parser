using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input
{
    [StructLayout(LayoutKind.Auto)]
    public readonly ref struct MatchInfo
    {
        public readonly ReadOnlySpan<char> Source;
        public readonly int Start;
        public readonly int Length;

        public bool SourceEndsWithThis => Start + Length == Source.Length;
        
        public MatchInfo(ReadOnlySpan<char> source, int start, int length)
        {
            Start = start;
            Length = length;
            Source = source;
        }

        public MatchInfo(ReadOnlySpan<char> source) : this(source, 0, source.Length) { }

        public ReadOnlySpan<char> SliceMatch() =>
            Source.Slice(Start, Length);

        public ReadOnlySpan<char> SliceRest() =>
            Source[(Start + Length)..];
    }
}
