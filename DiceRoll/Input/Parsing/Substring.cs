using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Substring
    {
        public readonly string Source;
        public readonly int Start;
        public readonly int Length;

        public int End => Start + Length;
        public int UntilSourceEnd => Source.Length - End;

        public bool IsEmpty => Length is 0;

        public char this[in int i] => IndexThis(in i);
        public char this[in Index i] => IndexThis(in i);
        public Substring this[in Range i] => IndexThis(in i);

        public Substring(string source, int start, int length)
        {
            ArgumentException.ThrowIfNullOrEmpty(source);
            ArgumentOutOfRangeException.ThrowIfLessThan(start, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(start + length, source.Length);
            
            Start = start;
            Length = length;
            Source = source;
        }

        public Substring(string source) : this(source, 0, source.Length) { }

        public Substring(in Substring source, int start, int length) : this(
            source.Source,
            source.Start + start,
            length
            ) { }

        public Substring(string source, in Range range) : this(source, range.GetOffsetAndLength(source.Length)) { }
        
        private Substring(string source, (int start, int length) tuple) : this(source, tuple.start, tuple.length) { }

        public Substring(in Substring source, in Range range) : this(
            in source,
            range.GetOffsetAndLength(source.Length)
            ) { }

        private Substring(in Substring source, (int start, int length) tuple) : this(
            in source,
            tuple.start,
            tuple.length
            ) { }

        public ReadOnlySpan<char> AsSpan() =>
            Source.AsSpan(Start, Length);
        public ReadOnlySpan<char> AsSpan(int start) =>
            MoveStart(start).AsSpan();
        public ReadOnlySpan<char> AsSpan(int start, int length) =>
            MoveStart(start).SetLength(length).AsSpan();

        public Range AsRange() =>
            new(Start, End);
        public Range AsRange(int start) =>
            MoveStart(start).AsRange();
        public Range AsRange(int start, int length) =>
            MoveStart(start).SetLength(length).AsRange();

        // todo get rid of these
        public int SourceIndexToRelativeIndex(int sourceIndex) =>
            sourceIndex - Start;
        public Range SourceRangeToRelativeRange(in Range sourceRange) =>
            SourceIndexToRelativeIndex(sourceRange.Start.GetOffset(Source.Length))
                ..
                SourceIndexToRelativeIndex(sourceRange.End.GetOffset(Source.Length));

        public Substring MoveStart(int offset) =>
            new(Source, Start + offset, Length - offset);

        public Substring MoveEnd(int offset) =>
            new(Source, Start, Length - offset);
        
        public Substring SetStart(int start) =>
            MoveStart(start - Start);

        public Substring SetEnd(int end) =>
            MoveEnd(End - end);

        public Substring SetLength(int newLength) =>
            new(Source, Start, newLength);

        public Substring Trim() =>
            TrimStart().TrimEnd();

        public Substring TrimStart()
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

        public Substring TrimEnd()
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

        public override string ToString() =>
            Source.Substring(Start, Length);

        public Enumerator GetEnumerator() =>
            new(this);

        public int IndexOf(string value, StringComparison stringComparison) =>
            IndexOf(value.AsSpan(), stringComparison);

        public int IndexOf(in Substring value, StringComparison stringComparison) =>
            IndexOf(value.AsSpan(), stringComparison);

        public int IndexOf(ReadOnlySpan<char> value, StringComparison stringComparison) =>
            AsSpan().IndexOf(value, stringComparison);

        private char IndexThis(in int i)
        {
            if (i < 0 || i >= Length)
                throw new IndexOutOfRangeException();
            
            return Source[Start + i];
        }

        private char IndexThis(in Index i) =>
            IndexThis(i.GetOffset(Length));

        private Substring IndexThis(in Range range)
        {
            (int offset, int length) = range.GetOffsetAndLength(Length);

            return new Substring(in this, offset, length);
        }

        public static Substring Empty(string source) =>
            new(source, 0, 0);

        public static Substring Empty(in Substring source) =>
            new(in source, 0, 0);

        public static Substring All(string source) =>
            new(source);

        public struct Enumerator
        {
            private readonly Substring _substring;
            private int _state;
            
            public char Current => _substring[_state];

            public Enumerator(Substring substring)
            {
                _substring = substring;
                _state = -1;
            }

            public bool MoveNext() =>
                ++_state < _substring.Length;
        }
    }

    public static class SubstringExtensions
    {
        public static bool IsEmptyOrWhiteSpace(this in Substring substring) =>
            substring.IsEmpty || substring.IsWhiteSpace();

        public static bool IsWhiteSpace(this in Substring substring)
        {
            foreach (char c in substring)
                if (!char.IsWhiteSpace(c))
                    return false;

            return true;
        }

        public static Substring Set(this in Substring substring, in Range range) =>
            substring.EnvelopSource()[in range];
        
        public static Substring EnvelopSource(this in Substring substring) =>
            Substring.All(substring.Source);
        
        public static Substring Empty(this in Substring substring) =>
            Substring.Empty(in substring);
    }
}
