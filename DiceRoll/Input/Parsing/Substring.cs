using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Substring : IEnumerable<char>
    {
        public readonly string Source;
        public readonly int Start;
        public readonly int Length;

        public int End => Start + Length;

        public bool IsEmpty => Length is 0;

        public char this[in int i] => IndexThis(in i);
        public char this[in Index i] => IndexThis(in i);

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

        public Substring(in Substring source, in Range range) : this(
            in source,
            range.GetOffsetAndLength(source.Length)
            ) { }

        private Substring(in Substring source, (int start, int length) tuple) : this(
            in source,
            tuple.start,
            tuple.length
            ) { }

        public override string ToString() =>
            Source.Substring(Start, Length);

        public Enumerator GetEnumerator() =>
            new(this);

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

        public static implicit operator Substring(string str) =>
            new(str);

        IEnumerator<char> IEnumerable<char>.GetEnumerator() =>
            GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        [StructLayout(LayoutKind.Auto)]
        public struct Enumerator : IEnumerator<char>
        {
            private readonly Substring _substring;
            private int _state;

            public char Current => _substring[_state];

            object IEnumerator.Current => Current;

            public Enumerator(in Substring substring)
            {
                _substring = substring;
                _state = -1;
            }

            public bool MoveNext() =>
                ++_state < _substring.Length;

            public void Dispose() { }

            void IEnumerator.Reset() =>
                throw new NotSupportedException();
        }
    }

    public static class SubstringExtensions
    {
        public static ReadOnlySpan<char> AsSpan(this Substring substring) =>
            substring.Source.AsSpan(substring.Start, substring.Length);

        public static ReadOnlySpan<char> AsSpan(this Substring substring, int start) =>
            substring.MoveStart(start).AsSpan();
        
        public static ReadOnlySpan<char> AsSpan(this Substring substring, int start, int length) =>
            substring.MoveStart(start).SetLength(length).AsSpan();

        public static Range AsRange(this Substring substring) =>
            substring.Start..substring.End;
        
        public static Range AsRange(this Substring substring, int start) =>
            substring.MoveStart(start).AsRange();
        
        public static Range AsRange(this Substring substring, int start, int length) =>
            substring.MoveStart(start).SetLength(length).AsRange();
        
        public static Substring MoveStart(this Substring substring, int offset) =>
            new(substring.Source, substring.Start + offset, substring.Length - offset);

        public static Substring MoveEnd(this Substring substring, int offset) =>
            new(substring.Source, substring.Start, substring.Length - offset);

        public static Substring SetRange(this Substring substring, in Range range) =>
            new(substring.Source, in range);

        public static Substring SetStart(this Substring substring, int start) =>
            substring.MoveStart(start - substring.Start);

        public static Substring SetEnd(this Substring substring, int end) =>
            substring.MoveEnd(substring.End - end);

        public static Substring SetLength(this Substring substring, int newLength) =>
            new(substring.Source, substring.Start, newLength);

        public static Substring Trim(this Substring substring) =>
            substring.TrimStart().TrimEnd();

        public static Substring TrimStart(this Substring substring)
        {
            int trim = 0;
            
            for (int i = 0; i < substring.Length; i++)
            {
                if (!char.IsWhiteSpace(substring[i]))
                    break;

                trim++;
            }

            return trim >= 0 ? substring.MoveStart(trim) : substring;
        }

        public static Substring TrimEnd(this Substring substring)
        {
            int trim = 0;
            
            for (int i = substring.Length - 1; i >= 0; i--)
            {
                if (!char.IsWhiteSpace(substring[i]))
                    break;

                trim++;
            }

            return trim >= 0 ? substring.MoveEnd(trim) : substring;
        }

        public static bool IsEmptyOrWhiteSpace(this Substring substring) =>
            substring.IsEmpty || substring.IsWhiteSpace();

        public static bool IsWhiteSpace(this Substring substring)
        {
            foreach (char c in substring)
                if (!char.IsWhiteSpace(c))
                    return false;

            return true;
        }

        public static int IndexOf(this Substring substring, string value, StringComparison stringComparison) =>
            substring.IndexOf(value.AsSpan(), stringComparison);

        public static int IndexOf(this Substring substring, in Substring value, StringComparison stringComparison) =>
            substring.IndexOf(value.AsSpan(), stringComparison);

        public static int IndexOf(this Substring substring, ReadOnlySpan<char> value, StringComparison stringComparison) =>
            substring.AsSpan().IndexOf(value, stringComparison);

        public static Substring Empty(this Substring substring) =>
            new(in substring, 0, 0);
    }
}
