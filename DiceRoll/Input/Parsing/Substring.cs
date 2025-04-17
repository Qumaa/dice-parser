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

        public bool Empty => Start == End;

        public char this[int i] => IndexThis(i);
        public char this[Index i] => IndexThis(i);
        public Substring this[Range i] => IndexThis(i);

        public Substring(string source, int start, int length)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentOutOfRangeException.ThrowIfLessThan(start, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(start + length, source.Length, nameof(length));
            
            Start = start;
            Length = length;
            Source = source;
        }

        public Substring(string source) : this(source, 0, source.Length) { }
        
        public Substring(Substring source, int start, int length) : this(source.Source, source.Start + start, length) { }

        public ReadOnlySpan<char> AsSpan() =>
            Source.AsSpan(Start, Length);
        public ReadOnlySpan<char> AsSpan(int start) =>
            MoveStart(start).AsSpan();
        public ReadOnlySpan<char> AsSpan(int start, int length) =>
            MoveStart(start).SetLength(length).AsSpan();

        public Substring MoveStart(int offset) =>
            new(Source, Start + offset, Length - offset);

        public Substring MoveEnd(int offset) =>
            new(Source, Start, Length - offset);

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

        private char IndexThis(int i)
        {
            if (i < 0 || i >= Length)
                throw new IndexOutOfRangeException();
            
            return Source[Start + i];
        }
        private char IndexThis(Index i) =>
            IndexThis(i.IsFromEnd ? Length - i.Value : i.Value);

        private Substring IndexThis(Range range)
        {
            (int Offset, int Length) tuple = range.GetOffsetAndLength(Length);

            return new Substring(this, Start + tuple.Offset, tuple.Length);
        }

        public override string ToString() =>
            AsSpan().ToString();

        public Enumerator GetEnumerator() =>
            new(this);

        public int IndexOf(string value, StringComparison stringComparison) =>
            IndexOf(value.AsSpan(), stringComparison);
        
        public int IndexOf(Substring value, StringComparison stringComparison) =>
            IndexOf(value.AsSpan(), stringComparison);

        public int IndexOf(ReadOnlySpan<char> value, StringComparison stringComparison) =>
            AsSpan().IndexOf(value, stringComparison);

        public static Substring None(string source) =>
            new(source, 0, 0);

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
}
