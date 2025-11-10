using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    public interface IToken
    {
        bool Matches(in Substring input, out Substring firstMatch);
    }

    public static class TokenExtensions
    {
        public static bool Matches(this IToken token, in Substring input) =>
            token.Matches(input, out _);

        public static MatchesEnumerable EnumerateMatches(this IToken token, in Substring input) =>
            new(token, in input);

        public static bool MatchesStart(this IToken token, in Substring input, out Substring matchSubstring) =>
            token.Matches(in input, out matchSubstring) && matchSubstring.Start == input.Start;
        
        public static bool MatchesStart(this IToken token, in Substring input) =>
            token.MatchesStart(in input, out _);
        
        public static bool MatchesAll(this IToken token, in Substring input, out Substring matchSubstring) =>
            token.MatchesStart(in input, out matchSubstring) && matchSubstring.End == input.End;
        
        public static bool MatchesAll(this IToken token, in Substring input) =>
            token.MatchesAll(in input, out _);

        public static bool MatchesEnd(this IToken token, in Substring input, out Substring matchSubstring)
        {
            using MatchesEnumerable.Enumerator enumerator = token.EnumerateMatches(input).GetEnumerator();
                
            if (!enumerator.MoveNext())
            {
                matchSubstring = input.Empty();
                return false;
            }

            do
                matchSubstring = enumerator.Current;
            while (enumerator.MoveNext());

            if (matchSubstring.End == input.End)
                return true;
            
            matchSubstring = input.Empty();
            return false;
        }

        public static bool MatchesEnd(this IToken token, in Substring input) =>
            token.MatchesEnd(in input, out _);

        [StructLayout(LayoutKind.Auto)]
        public readonly struct MatchesEnumerable : IEnumerable<Substring>
        {
            private readonly IToken _token;
            private readonly Substring _substring;
            
            public MatchesEnumerable(IToken token, in Substring substring)
            {
                _substring = substring;
                _token = token;
            }

            [StructLayout(LayoutKind.Auto)]
            public struct Enumerator : IEnumerator<Substring>
            {
                private readonly IToken _token;
                private readonly int _end;
                private Substring _current;

                public Substring Current => _current;

                object IEnumerator.Current => Current;

                public Enumerator(IToken token, in Substring substring)
                {
                    _current = substring.Empty();
                    _end = substring.End;
                    _token = token;
                }

                public bool MoveNext()
                {
                    Substring substring = new(_current.Source, _current.End, _end - _current.End);
                    
                    return _token.Matches(in substring, out _current);
                }

                public void Reset() =>
                    throw new System.NotSupportedException();

                public void Dispose() { }
            }

            public Enumerator GetEnumerator() =>
                new(_token, in _substring);

            IEnumerator<Substring> IEnumerable<Substring>.GetEnumerator() =>
                GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() =>
                GetEnumerator();
        }
    }
}
