using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input
{
    // todo: rename to SubstringPointer & covert Source to string
    [StructLayout(LayoutKind.Auto)]
    public readonly ref struct MatchInfo
    {
        public readonly ReadOnlySpan<char> Source;
        public readonly int Start;
        public readonly int Length;

        public int End => Start + Length;
        public int UntilSourceEnd => Source.Length - End;

        public bool Empty => Start == Source.Length;

        public char this[int i] => IndexThis(i);

        public MatchInfo(ReadOnlySpan<char> source, int start, int length)
        {
            Start = start;
            Length = length;
            Source = source;
        }

        public MatchInfo(ReadOnlySpan<char> source) : this(source, 0, source.Length) { }

        public ReadOnlySpan<char> SliceMatch() =>
            Source.Slice(Start, Length);

        public ReadOnlySpan<char> SliceAfter() =>
            Source[End..];

        public MatchInfo After()
        {
            int afterStart = Start + Length;
            
            return new MatchInfo(Source, afterStart, Source.Length - afterStart);
        }

        public MatchInfo Before() =>
            new(Source, 0, Start);

        public MatchInfo MoveStart(int offset) =>
            new(Source, Start + offset, Length - offset);

        public MatchInfo MoveEnd(int offset) =>
            new(Source, Start, Length - offset);

        public MatchInfo Trim() =>
            TrimStart().TrimEnd();

        public MatchInfo TrimStart()
        {
            int trim = 0;
            
            for (int i = 0; i < Length; i++)
            {
                if (!char.IsWhiteSpace(this[i]))
                    break;

                trim++;
            }

            return trim >= 0 ? MoveStart(trim) : this;
        }

        public MatchInfo TrimEnd()
        {
            int trim = 0;
            
            for (int i = Length - 1; i >= 0; i--)
            {
                if (!char.IsWhiteSpace(this[i]))
                    break;

                trim++;
            }

            return trim >= 0 ? MoveEnd(trim) : this;
        }

        public Boxed Box() =>
            new(this);

        private char IndexThis(int i)
        {
            if (i < 0 || i >= Length)
                throw new IndexOutOfRangeException();
            
            return Source[Start + i];
        }

        public override string ToString() =>
            SliceMatch().ToString();

        public static MatchInfo None(ReadOnlySpan<char> source) =>
            new(source, 0, 0);
        
        public static MatchInfo All(ReadOnlySpan<char> source) =>
            new(source);

        [StructLayout(LayoutKind.Auto)]
        public readonly struct Boxed
        {
            private readonly string _source;
            private readonly int _start;
            private readonly int _length;

            public Boxed(in MatchInfo matchInfo)
            {
                _source = matchInfo.Source.ToString();
                _start = matchInfo.Start;
                _length = matchInfo.Length;
            }

            public MatchInfo Unbox() =>
                new(_source, _start, _length);
        }
    }
}
